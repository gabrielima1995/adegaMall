using Application.Config;
using Application.Modules.Finance;
using Domain;
using Domain.Services.StoreModule;
using Framework;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using V2.Application.Modules.Finance;

namespace Presentation.Controllers
{
    [AuthorizeMaster]
    public class InternalController : BaseController
    {
        private const string Key = "7uSQueUG4uhyQWwT";

        private readonly DoTransfersRecurrentJob doTransfersDailyJob;
        private readonly OrderService orderService;
        private readonly ResendGatewayIds jobOnUpdateApiKey;
        private readonly IRepository<ApplicationConfig> appConfigRepository;

        public InternalController(
            DoTransfersRecurrentJob doTransfersDailyJob,
            OrderService orderService,
            ResendGatewayIds resendGatewayIds,
            IRepository<ApplicationConfig> appConfigRepository)
        {
            this.doTransfersDailyJob = doTransfersDailyJob;
            this.orderService = orderService;
            this.jobOnUpdateApiKey = resendGatewayIds;
            this.appConfigRepository = appConfigRepository;
        }

        public ActionResult DoTransfers(string key)
        {
            if (key != Key) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            doTransfersDailyJob.Run();
            return Content("OK");
        }

        public ActionResult EmulateGatewayTransactionPostback(string key, string transactionId)
        {
            if (key != Key) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (transactionId == null) return Content("Bad Request, missing parameter " + nameof(transactionId));
            orderService.ProcessGatewayPostback(transactionId);
            return Content("OK");
        }

        public ActionResult ReRegisterUsersOnGateway(string key)
        {
            if (key != Key) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            jobOnUpdateApiKey.Run();
            return Content("OK");
        }

        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult UpdateConfig(string key)
        {
            if (key != Key) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var appUrl = Request.Form["AppUrl"];
            var cdnUrl = Request.Form["CdnUrl"];

            var storageEndpoint = Request.Form["StorageEndpoint"];
            var storageUsername = Request.Form["StorageUsername"];
            var storagePassword = Request.Form["StoragePassword"];

            var smtpHost = Request.Form["SmtpHost"];
            var smtpPort = Request.Form["SmtpPort"];
            var smtpUsername = Request.Form["SmtpUsername"];
            var smtpPassword = Request.Form["SmtpPassword"];
            var smtpUseSsl = Request.Form["SmtpUseSsl"];
            var smtpFromAddress = Request.Form["SmtpFromAddress"];
            var smtpFromName = Request.Form["SmtpFromName"];

            var appConfig = appConfigRepository.Query().FirstOrDefault();
            var config = MaaSConfig.FromJson(appConfig.MaaSConfig);

            #region App URL + CDN URL

            if (appUrl != null)
                config.Environment.AppUrl = appUrl;

            if (cdnUrl != null)
            {
                config.Cdn.Endpoint = cdnUrl;
                config.Cdn.Enabled = true;
            }

            #endregion

            #region Storage

            if (storageEndpoint != null)
                config.Environment.CloudStorage.Endpoint = storageEndpoint;

            if (storageUsername != null)
                config.Environment.CloudStorage.Username = storageUsername;

            if (storagePassword != null)
                config.Environment.CloudStorage.Password = storagePassword;

            #endregion

            #region SMTP

            if (smtpHost != null)
                config.Environment.Smtp.Host = smtpHost;

            if (smtpPort != null)
            {
                if (!int.TryParse(smtpPort, out int port))
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "SMTP Port must be an integer.");

                config.Environment.Smtp.Port = port;
            }

            if (smtpUsername != null)
                config.Environment.Smtp.Username = smtpUsername;

            if (smtpPassword != null)
                config.Environment.Smtp.Password = smtpPassword;

            if (smtpUseSsl != null)
            {
                if (!bool.TryParse(smtpUseSsl, out bool useSsl))
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "SMTP UseSSL must be a boolean value.");

                config.Environment.Smtp.UseSsl = useSsl;
            }

            if (smtpFromAddress != null)
            {
                try
                {
                    Guard.Ensure.IsValidEmail(smtpFromAddress, nameof(smtpFromAddress));
                    config.Environment.Smtp.DefaultFromAddress = smtpFromAddress;
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway, "SMTP From Address must be a valid email address.");
                }
            }

            if (smtpFromName != null)
                config.Environment.Smtp.DefaultFromName = smtpFromName;

            #endregion

            var errors = ConfigValidator.CheckErrors(config).Errors;

            if (errors.HasErrors)
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway, errors.ToSimpleString());

            appConfig.MaaSConfig = config.ToJson();
            appConfigRepository.Update(appConfig);
            appConfigRepository.SaveChanges();

            MaaSConfig.Load(appConfig);

            return Content("Config successfully updated.");
        }
    }
}