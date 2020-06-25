using Dapper;
using Domain.Data.SqlScripts;
using Domain.DTO;
using Domain.Entities;
using Domain.Events;
using Domain.Events.PortfolioModule;
using Domain.Events.SellerProfileUpdated;
using Domain.Events.Subscriptions;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.SellerAssistantModule;
using Domain.Modules.StoreModule.CompleteProfile;
using Framework;
using Infrastructure.Data.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories
{
    public class PersonRepository : EfCrudRepository<Person>, IPersonRepository
    {
        public PersonRepository(ScaffoldContext context) : base(context) { }

        public LoginProfile GetLoginProfile(string userId, string personId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript(ScriptName.PersonRepository_GetLoginProfile);
                var grid = conn.QueryMultiple(sql, new { userId, personId });

                var profile = grid.ReadSingleOrDefault<LoginProfileAux>();
                profile.AssistantPermissions = grid.Read<string>().ToList().AsReadOnly();

                return profile;
            }
        }

        public ReadOnlyCollection<AdminPerson> ListAdminPersons()
        {
            using (var conn = MyDbConnection.Create())
            {
                var sqlScript = "SELECT * FROM Person WHERE Discriminator = 'AdminPerson' AND Enabled = 1 AND Email != 'marketplace@ideianoar.com.br'";
                var results = conn.Query<AdminPerson>(sqlScript).ToList();
                return results.AsReadOnly();
            }
        }

        public Person GetByUsername(string username)
        {
            return Context.Set<Person>().FirstOrDefault(person => person.UserName == username);
        }

        public string GetPersonIdForUser(string userId, string preferredPersonId = null)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    DECLARE @personId VARCHAR(128)

                    SELECT TOP 1 @personId = Person.Id
                    FROM Person
                    JOIN AspNetUsers ON Person.UserId = AspNetUsers.Id AND AspNetUsers.Enabled = 1
                    WHERE Person.UserId = @userId AND (@preferredId IS NULL OR Person.Id = @preferredId)
                    ORDER BY CreatedAt

                    IF (@personId IS NULL) BEGIN
                        SELECT TOP 1 @personId = Person.Id
                        FROM Person
                        JOIN AspNetUsers ON Person.UserId = AspNetUsers.Id AND AspNetUsers.Enabled = 1
                        WHERE Person.UserId = @userId
                        ORDER BY CreatedAt
                    END

                    IF (@personId IS NULL) BEGIN
                        SELECT TOP 1 @personId = Person.Id
                        FROM Person
                        JOIN AspNetUsers ON Person.UserId = AspNetUsers.Id
                        WHERE Person.UserId = @userId
                        ORDER BY CreatedAt
                    END

                    SELECT @personId";

                return conn.QuerySingleOrDefault<string>(sql, new
                {
                    userId,
                    preferredId = preferredPersonId
                });
            }
        }

        private class LoginProfileAux : LoginProfile
        {
            public string SellerCompleteProfileJson
            {
                set
                {
                    if (value != null)
                        SellerCompleteProfile = JsonConvert.DeserializeObject<SellerCompleteProfile>(value);
                }
            }
        }
    }

    public class CachingPersonRepository : IPersonRepository,
        IDomainEventHandler<PortfolioCreatedEvent>,
        IDomainEventHandler<PortfolioItemAddedEvent>,
        IDomainEventHandler<PortfolioItemRemovedEvent>,
        IDomainEventHandler<PortfolioItemUpdatedEvent>,
        IDomainEventHandler<PortfolioUpdatedEvent>,
        IDomainEventHandler<SubscriptionAddedEvent>,
        IDomainEventHandler<SubscriptionUpdatedEvent>,
        IDomainEventHandler<SubscriptionCancelledEvent>,
        IDomainEventHandler<PersonUpdatedEvent>,
        IDomainEventHandler<BuyerUpdatedEvent>,
        IDomainEventHandler<SellerUpdatedEvent>,
        IDomainEventHandler<SellerProfileUpdated>,
        IDomainEventHandler<SellerAssistantPermissionsSet>
    {
        private readonly IPersonRepository wrappedRepository;
        private readonly ICache cache;

        public CachingPersonRepository(IPersonRepository wrappedRepository, ICache cache)
        {
            this.wrappedRepository = wrappedRepository;
            this.cache = cache;
        }

        public LoginProfile GetLoginProfile(string userId, string personId)
        {
            var key = GetLoginProfileKey(userId, personId);

            if (!cache.Contains(key))
            {
                var value = wrappedRepository.GetLoginProfile(userId, personId);
                cache.Add(key, value, TimeSpan.FromMinutes(5), CacheTimeoutType.Sliding);
            }

            return cache.Get<LoginProfile>(key);
        }
        public string GetPersonIdForUser(string userId, string preferredPersonId = null)
        {
            var key = $"PersonIdForUser_{userId}_{preferredPersonId}";

            if (!cache.Contains(key))
            {
                var value = wrappedRepository.GetPersonIdForUser(userId, preferredPersonId);
                cache.Add(key, value, TimeSpan.FromMinutes(5), CacheTimeoutType.Sliding);
            }

            return cache.Get<string>(key);
        }

        public void Handle(PersonUpdatedEvent domainEvent) => RemoveCache(domainEvent.PersonId);
        public void Handle(SubscriptionAddedEvent domainEvent) => RemoveCache(domainEvent.PersonId);
        public void Handle(SubscriptionUpdatedEvent domainEvent) => RemoveCache(domainEvent.PersonId);
        public void Handle(SubscriptionCancelledEvent domainEvent) => RemoveCache(domainEvent.Subscription.SellerId);
        public void Handle(PortfolioCreatedEvent domainEvent) => RemoveCache(domainEvent.Portfolio.SellerId);
        public void Handle(PortfolioItemAddedEvent domainEvent) => RemoveCache(domainEvent.SellerId);
        public void Handle(PortfolioItemRemovedEvent domainEvent) => RemoveCache(domainEvent.SellerId);
        public void Handle(PortfolioItemUpdatedEvent domainEvent) => RemoveCache(domainEvent.SellerId);
        public void Handle(PortfolioUpdatedEvent domainEvent) => RemoveCache(domainEvent.Portfolio.SellerId);
        public void Handle(BuyerUpdatedEvent domainEvent) => RemoveCache(domainEvent.BuyerId);
        public void Handle(SellerUpdatedEvent domainEvent) => RemoveCache(domainEvent.SellerId);
        public void Handle(SellerProfileUpdated domainEvent) => RemoveCache(domainEvent.SellerId);
        public void Handle(SellerAssistantPermissionsSet domainEvent) => RemoveCache(domainEvent.AssistantPersonId);
        private void RemoveCache(string userOrPersonId) => cache.Remove(GetLoginProfileKey(userOrPersonId, null));
        private string GetLoginProfileKey(string userId, string personId) => $"LoginProfile_{personId ?? userId}";

        #region Not handled

        public OperationResult Add(Person entity, bool saveChanges = false) => wrappedRepository.Add(entity, saveChanges);
        public OperationResult Delete(Person entity, bool saveChanges = false) => wrappedRepository.Delete(entity, saveChanges);
        public Person Get(Expression<Func<Person, bool>> predicate, params Expression<Func<Person, object>>[] includes) => wrappedRepository.Get(predicate, includes);
        public Person GetById(params object[] idValues) => wrappedRepository.GetById(idValues);
        public Person GetByUsername(string username) => wrappedRepository.GetByUsername(username);
        public List<Person> List(params Expression<Func<Person, object>>[] includes) => wrappedRepository.List(includes);
        public ReadOnlyCollection<AdminPerson> ListAdminPersons() => wrappedRepository.ListAdminPersons();
        public OperationResult SaveChanges() => wrappedRepository.SaveChanges();
        public OperationResult Update(Person entity, bool saveChanges = false) => wrappedRepository.Update(entity, saveChanges);

        #endregion
    }
}