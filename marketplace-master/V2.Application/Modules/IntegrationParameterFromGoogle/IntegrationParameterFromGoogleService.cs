using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using System.Collections.Generic;
using Infrastructure.Data.Repositories;
using System.Threading.Tasks;
using System.Text;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2_1;
using Google.Apis.ShoppingContent.v2_1.Data;

namespace V2.Application.Modules.IntegrationParameterFromGoogle
{
    public class IntegrationParameterFromGoogleService
    {
        ProductRepository _productRepository;
        IntegrationParameterFromGoogleRepository _integrationGoogleRepository;
        private static readonly string endpointEnvVar = "GOOGLE_SHOPPING_SAMPLES_ENDPOINT";
        protected ShoppingContentService service;
        protected MerchantConfig config;
        protected ShoppingContentService sandboxService;

        #region Autenticacao
        private bool Autenticar()
        {
            try
            {
                var ListaParametro = _integrationGoogleRepository.List(true);

                if (ListaParametro == null || ListaParametro?.Count == 0)
                {
                    return false;
                }

                StringBuilder sbServiceAccount = new StringBuilder();

                sbServiceAccount.Append("{");
                sbServiceAccount.Append("  \"type\": \"service_account\",");
                sbServiceAccount.Append($"  \"private_key\": \"{ListaParametro[0].PrivateKey}\",");
                sbServiceAccount.Append($"  \"client_email\": \"{ListaParametro[0].ClientEmail}\",");
                sbServiceAccount.Append($"  \"client_id\": \" + {ListaParametro[0].ClientId} \"");
                sbServiceAccount.Append("}");

                string userId = ListaParametro[0].AccountId;
                string scope = "https://www.googleapis.com/auth/content";
                String[] scopes = new[] { scope };

                MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(sbServiceAccount.ToString()));

                GoogleCredential credential = GoogleCredential.FromStream(ms);
                var initializer = credential.CreateScoped(scopes);

                if (initializer == null)
                {
                    return false;
                }

                var init = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = initializer,
                    ApplicationName = $"RichBeauty",
                };

                if (Environment.GetEnvironmentVariable(endpointEnvVar) == null)
                {
                    initializeService(init);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region initializeService
        internal void initializeService(BaseClientService.Initializer init)
        {
            config = new MerchantConfig();
            service = new ShoppingContentService(init);
            retrieveConfiguration(service, config);
        }
        #endregion

        #region retrieveConfiguration

        internal void retrieveConfiguration(ShoppingContentService service, MerchantConfig config)
        {
            var authinfo = service.Accounts.Authinfo().Execute();

            if (authinfo.AccountIdentifiers.Count == 0)
            {
                return;
                //throw new ArgumentException(
                //    "Authenticated user has no access to any Merchant Center accounts.");
            }
            if (config.MerchantId == null)
            {
                var firstAccount = authinfo.AccountIdentifiers[0];
                if (firstAccount.MerchantId == null)
                {
                    config.MerchantId = firstAccount.AggregatorId;
                }
                else
                {
                    config.MerchantId = firstAccount.MerchantId;
                }
            }

            ulong merchantId = config.MerchantId.Value;

            config.IsMCA = false;

            foreach (var accountId in authinfo.AccountIdentifiers)
            {
                if (merchantId == accountId.AggregatorId)
                {
                    config.IsMCA = true;
                    break;
                }
                if (merchantId == accountId.MerchantId)
                {
                    break;
                }
            }

            var account = service.Accounts.Get(merchantId, merchantId).Execute();
            if (!String.IsNullOrEmpty(account.WebsiteUrl))
            {
                config.WebsiteURL = account.WebsiteUrl;
            }
            else
            {
                return;
            }
        }

        #endregion

        #region IntegrationParameterFromGoogleService
        public IntegrationParameterFromGoogleService(Infrastructure.Data.Repositories.ProductRepository produto, IntegrationParameterFromGoogleRepository integrationGoogle)
        {
            _productRepository = produto;
            _integrationGoogleRepository = integrationGoogle;
        }
        #endregion

        #region Run
        public void Run()
        {
            if (!Autenticar())
            {
                return;
            }

            List<Product> ListProduct = new List<Product>();
            Product product = new Product();

            var ListaProdutoIntegrar = _productRepository.ListProdutoMustBeIntegrated();

            foreach (var item in ListaProdutoIntegrar)
            {
                product = new Product();
                product.OfferId = item.Id.ToString();
                product.Title = item.Title;
                product.Description = item.Description;
                product.Link = $"https://www.adegamall.com.br";
                product.ImageLink = item.CoverImageUrl;

                if (item.Images?.Count > 0)
                {
                    foreach (var itemImagem in item.Images)
                    {
                        product.AdditionalImageLinks.Add(itemImagem.Url);
                    }
                }

                product.Availability = "limited availability";
                product.Brand = "nenhuma";
                product.Condition = "new";
                product.Channel = "online";
                product.ContentLanguage = "PT";
                product.TargetCountry = "BR";
                product.Condition = "new";
                product.Price = new Price();
                product.Price.Currency = "BRL";
                product.Price.Value = item.Price.ToString();
                product.Price.Value = product.Price.Value.Replace(",", ".");

                ListProduct.Add(product);
            }

            foreach (var item in ListProduct)
            {

                try
                {
                    InsertProduct(ulong.Parse(config.MerchantId.ToString()), "https://www.adegamall.com.br", item);

                    var prodUpdate = ListaProdutoIntegrar.Find(a => a.Id.Equals(Guid.Parse(item.OfferId)));
                    prodUpdate.LastIntegration = DateTime.Now;
                    _productRepository.UpdateAndSave(prodUpdate);
                }
                catch
                {
                    //Não faz o update na tabela   
                }
            }

        }
        #endregion

        #region InsertProduct
        private Product InsertProduct(ulong merchantId, string websiteUrl, Product product)
        {
            Product response = service.Products.Insert(product, merchantId).Execute();
            return response;
        }
        #endregion
    }
}
