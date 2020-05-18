using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Infra.Crosscutting.PayPal.Buttons
{
    public class PayPalButtonResult : ActionResult
    {
        private const string ActionTemplate = "https://{0}paypal.com/cgi-bin/webscr";

        private readonly string _action;
        private readonly bool _autoPost;
        private readonly string _messageAutoPost;

        private readonly IDictionary<string, string> _variables = new Dictionary<string, string>();

        public PayPalButtonResult(IPayPalConfig config, string cmd, IDictionary<string, string> variables, bool autoPost = false, string messageAutoPost = "Processing, please wait...")
        {
            if (variables != null) _variables = variables;

            _autoPost = autoPost;
            _messageAutoPost = messageAutoPost;

            AddVariable("cmd", cmd);
            AddVariable("business", config.MerchantId);
            AddVariable("notify_url", config.NotifyUrl);

            _action = string.Format(ActionTemplate, config.IsSandbox ? "www.sandbox." : "www.");
        }

        protected void AddVariable(string key, string value)
        {
            _variables.Add(key, value);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.Clear();

            var sb = new StringBuilder();

            if (_autoPost)
            {
                sb.Append("<html>");
                sb.Append(@"<body onload='document.forms[""form""].submit()'>");
                sb.Append($"<label>{_messageAutoPost}</label>");
            }

            sb.Append($"<form name='form' action='{_action}' method='post'>");

            foreach (var variable in _variables)
            {
                sb.Append($"<input type='hidden' name='{variable.Key}' value='{variable.Value}'>");
            }

            if (!_autoPost)
            {
                sb.Append(@"<input type=""image"" src=""https://www.paypalobjects.com/en_US/i/btn/btn_subscribe_LG.gif"" border=""0"" name=""submit""");
            }

            sb.Append("</form>");

            if (_autoPost)
            {
                sb.Append("</body>");
                sb.Append("</html>");
            }

            response.Write(sb.ToString());

            response.End();
        }
    }
}