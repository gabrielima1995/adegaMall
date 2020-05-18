using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System;
using System.IO;
using System.Text;

namespace PaymentIntegrator.Moip.Helpers
{
    public static class EncryptionHelper
    {
        //public static string EncryptString(string str)
        //{
        //    return null;
        //}

        private static string RsaEncryptWithPublic(string clearText, string publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(publicKey))
            {
                var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, keyParameter);
            }

            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;

        }

        public static string EncryptCard(object cardObject)
        {
            dynamic card = cardObject;

            string hashString = $"number={card.number.Replace(" ", "")}&cvc={card.cvc}&expirationMonth={card.expirationMonth}&expirationYear={card.expirationYear}";

            return RsaEncryptWithPublic(hashString, MoipConfig.Current.PublicEncryptionKey);
        }
    }
}
