namespace V2.Application.Common
{
    public class TransientBuyer
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public TransientBuyer(string name, string email, string password = null)
        {
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
