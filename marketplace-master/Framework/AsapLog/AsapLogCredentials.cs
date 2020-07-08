namespace Framework.AsapLog
{
    public class AsapLogCredentials
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }

        public bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Senha));
        }
    }
}