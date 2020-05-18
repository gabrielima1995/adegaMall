namespace V2.Application.Modules.PushNotifications
{
    public interface IPushNotificationService
    {
        void NotifyUser(string userId, PushNotification message);

        void NotifyByRole(string role, PushNotification message);

        void NotifyBybyRoleUserAndSeller(string role, PushNotification message);
    }
}