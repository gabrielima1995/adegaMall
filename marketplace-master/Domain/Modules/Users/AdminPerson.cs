namespace Domain.Entities
{
    public class AdminPerson : Person
    {
        private AdminPerson() { }
        public AdminPerson(string userId, string email, string fullName) : base(userId, email, false)
        {
            FullName = fullName;
        }

        public override PersonType PersonType => PersonType.AdminPerson;
    }
}