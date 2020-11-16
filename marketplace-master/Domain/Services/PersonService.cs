using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using System.Linq;

namespace Domain
{
    public class PersonService : ServiceBase<Person>
    {
        private readonly IPersonRepository repository;

        public PersonService(
            IPersonRepository repository,
            IRepository<Person> oldRepository) : base(oldRepository)
        {
            this.repository = repository;
        }

        public bool ExistsByEmail(string email, string ownerUserId = null)
        {
            return Repository.Query().Any(person => person.Email == email && (ownerUserId == null || person.UserId != ownerUserId));
        }

        public bool ExistsByDocumentNumber(string documentNumber, string ownerUserId = null)
        {
            return Repository.Query().Any(person => person.DocumentNumber == documentNumber && (ownerUserId == null || person.UserId != ownerUserId));
        }

        public void SetUtmTagsIfNull(string username, string utmTags)
        {
            if (string.IsNullOrEmpty(utmTags))
                return;

            var person = repository.GetByUsername(username);

            if (person != null && person.UtmTags == null)
            {
                person.UtmTags = utmTags;
                repository.SaveChanges();
            }
        }
    }
}