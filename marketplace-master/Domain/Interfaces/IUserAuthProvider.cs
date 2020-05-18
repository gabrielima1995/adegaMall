using Domain.DTO;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserAuthProvider
    {
        bool HasValue { get; }
        string UserId { get; }
        string PersonId { get; }

        LoginProfile GetLoginProfile();
        ApplicationUser GetUser();
        void Set(string userId, string personId);
    }
}