using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace V2.Infrastructure.Auth
{
    public class UserAuthProvider : IUserAuthProvider
    {
        private readonly IPersonRepository personRepository;
        private readonly ScaffoldContext context;

        private ApplicationUser user;

        public UserAuthProvider(IPersonRepository personRepository, ScaffoldContext context)
        {
            this.personRepository = personRepository;
            this.context = context;
        }

        public bool HasValue => UserId != null;
        public string PersonId { get; private set; }
        public string UserId { get; private set; }

        public ApplicationUser GetUser()
        {
            if (UserId == null) return null;
            return user ?? (user = context.Users.Find(UserId));
        }

        public LoginProfile GetLoginProfile()
        {
            if (UserId == null) return null;
            return personRepository.GetLoginProfile(UserId, PersonId) ?? new LoginProfile();
        }

        public void Set(string userId, string personId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(personId))
                userId = personId = null;

            UserId = userId;
            PersonId = personId;
        }
    }
}