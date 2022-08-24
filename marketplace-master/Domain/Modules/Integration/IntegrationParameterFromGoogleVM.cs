using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Modules.Integration
{
    public class IntegrationParameterFromGoogleVM
    {
        public Guid Id { get; set; }

        //public String Seller_Id { get; set; }

        [DisplayNameLocalized("ID da conta do cliente (account_id)")]
        [RequiredLocalized]
        public String AccountId { get; set; }

        [DisplayNameLocalized("ID do cliente (client_id)")]
        [RequiredLocalized]
        public String ClientId { get; set; }

        [DisplayNameLocalized("E-mail do cliente (client_email)")]
        [RequiredLocalized]
        public String ClientEmail { get; set; }

        [DisplayNameLocalized("Chave privada do cliente (private_key)")]
        [RequiredLocalized]
        public String PrivateKey { get; set; }

        [DisplayNameLocalized("Ativo")]
        [RequiredLocalized]
        public bool Enabled { get; set; } = true;


        public IntegrationParameterFromGoogleVM()
        {
        }

        public IntegrationParameterFromGoogleVM(IntegrationParameterFromGoogle param)
        {
            this.Id = param.Id;
            //this.Seller_Id = param.Seller_Id;
            this.AccountId = param.AccountId;
            this.ClientId = param.ClientId;
            this.ClientEmail = param.ClientEmail;
            this.PrivateKey = param.PrivateKey;
            this.Enabled = param.Enabled;
        }

        public IntegrationParameterFromGoogle ToClass()
        {
            var param = new IntegrationParameterFromGoogle();

            if (this.Id == null || String.IsNullOrWhiteSpace(this.Id.ToString()) || this.Id.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                param.Id = Guid.NewGuid();
            }
            else
            {
                param.Id = Guid.Parse(this.Id.ToString().ToUpper());
            }

            //param.Seller_Id = this.Seller_Id;
            param.AccountId = this.AccountId;
            param.ClientId = this.ClientId;
            param.ClientEmail = this.ClientEmail;
            param.PrivateKey = this.PrivateKey;
            param.Enabled = this.Enabled;

            return param;
        }

        public IntegrationParameterFromGoogleVM ToClassVM()
        {
            var param = new IntegrationParameterFromGoogleVM();

            if (this.Id == null || String.IsNullOrWhiteSpace(this.Id.ToString()) || this.Id.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                param.Id = Guid.NewGuid();
            }
            else
            {
                param.Id = Guid.Parse(this.Id.ToString().ToUpper());
            }

            //param.Seller_Id = this.Seller_Id;
            param.AccountId = this.AccountId;
            param.ClientId = this.ClientId;
            param.ClientEmail = this.ClientEmail;
            param.PrivateKey = this.PrivateKey;
            param.Enabled = this.Enabled;

            return param;
        }
    }
}
