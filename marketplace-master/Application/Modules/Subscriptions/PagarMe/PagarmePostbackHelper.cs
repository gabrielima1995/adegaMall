using Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Application.Modules.Subscriptions.PagarMe
{
    public class PagarmePostbackHelper 
    {
        public OperationResult<PagarMeSubscriptionPostback> MapPostbackToSubscriptionPostbackData(string postbackQuery)
        {
            try
            {
                NameValueCollection postbackDataCollection = HttpUtility.ParseQueryString(postbackQuery);
                Dictionary<string, string> postbackDictonary = new Dictionary<string, string>();
                if (!postbackDataCollection.HasKeys())
                    return OperationResult<PagarMeSubscriptionPostback>.Fail("", "the postback return dont has any values");

                foreach (var item in postbackDataCollection.AllKeys)
                {
                    var value = postbackDataCollection[item];
                    postbackDictonary.Add(item.Trim(), value);
                }

                PagarMeSubscriptionPostback subscriptionPostbackData = new PagarMeSubscriptionPostback()
                {
                    id = postbackDictonary["subscription[current_transaction][id]"].ToString(),
                    fingerprint = postbackDictonary["fingerprint"].ToString(),
                    @event = postbackDictonary["event"].ToString(),
                    old_status = postbackDictonary["old_status"].ToString(),
                    desired_status = postbackDictonary["desired_status"].ToString(),
                    current_status = postbackDictonary["current_status"].ToString(),

                };

                subscriptionPostbackData.subscription = new SubscriptionPostbackData()
                {
                    id = postbackDataCollection["id"].ToString(),
                    postback_url = postbackDictonary["subscription[postback_url]"].ToString(),
                    payment_method = postbackDictonary["subscription[payment_method]"].ToString(),
                    current_period_start = postbackDictonary["subscription[current_period_start]"].ToString() != null &&
                                           !String.IsNullOrWhiteSpace(postbackDictonary["subscription[current_period_start]"].ToString())
                                              ? DateTime.Parse(postbackDictonary["subscription[current_period_start]"].ToString())
                                              : DateTime.UtcNow,
                    current_period_end = postbackDictonary["subscription[current_period_end]"].ToString() != null &&
                                         !String.IsNullOrWhiteSpace(postbackDictonary["subscription[current_period_end]"].ToString())
                                              ? DateTime.Parse(postbackDictonary["subscription[current_period_end]"].ToString())
                                              : DateTime.UtcNow,
                    charges = postbackDictonary["subscription[charges]"].ToString() != null &&
                              !String.IsNullOrWhiteSpace(postbackDictonary["subscription[charges]"].ToString())
                                   ? int.Parse(postbackDictonary["subscription[charges]"].ToString())
                                   : 0,
                    status = postbackDictonary["subscription[payment_method]"].ToString(),
                    date_created = postbackDictonary["subscription[plan][date_created]"].ToString() != null &&
                                   !String.IsNullOrWhiteSpace(postbackDictonary["subscription[plan][date_created]"].ToString())
                                              ? DateTime.Parse(postbackDictonary["subscription[plan][date_created]"].ToString())
                                              : DateTime.UtcNow,


                };

                subscriptionPostbackData.subscription.plan = new PlanPostbackData()
                {
                    id = postbackDictonary["subscription[plan][id]"].ToString(),
                    charges = postbackDictonary["subscription[plan][charges]"].ToString() != null &&
                              !String.IsNullOrWhiteSpace(postbackDictonary["subscription[plan][charges]"].ToString())
                                   ? int.Parse(postbackDictonary["subscription[plan][charges]"].ToString())
                                   : 0,
                    amount = int.Parse(postbackDictonary["subscription[plan][amount]"].ToString()),
                    name = postbackDictonary["subscription[plan][name]"].ToString(),
                    days = int.Parse(postbackDictonary["subscription[plan][days]"].ToString()),
                    trial_days = int.Parse(postbackDictonary["subscription[plan][trial_days]"].ToString())
                };

                subscriptionPostbackData.subscription.current_transaction = new TransactionPostbackData()
                {
                    id = postbackDictonary["subscription[current_transaction][id]"].ToString(),
                    amount = int.Parse(postbackDictonary["subscription[current_transaction][amount]"].ToString()),
                    payment_method = postbackDictonary["subscription[current_transaction][payment_method]"].ToString(),
                    status = postbackDictonary["subscription[current_transaction][status]"].ToString()
                };


                return subscriptionPostbackData;
            }
            catch (Exception ex)
            {
                return OperationResult<PagarMeSubscriptionPostback>.Fail("", ex.Message);
            }
        }
    }
}
