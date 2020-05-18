using System.ComponentModel.DataAnnotations;
using V2.Application.Modules.Admin;

namespace Presentation.Models
{
    public class PushNotificationViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public string Link { get; set; }

        [Required]
        public NotificationType TargetRole { get; set; }
    }
}