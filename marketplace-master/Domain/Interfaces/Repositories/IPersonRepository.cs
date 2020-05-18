using Domain.DTO;
using Domain.Entities;
using System.Collections.ObjectModel;

namespace Domain.Interfaces.Repositories
{
    public interface IPersonRepository : ICrudRepository<Person>
    {
        Person GetByUsername(string username);
        LoginProfile GetLoginProfile(string userId, string personId);
        ReadOnlyCollection<AdminPerson> ListAdminPersons();
        string GetPersonIdForUser(string userId, string preferredPersonId = null);
    }
}
