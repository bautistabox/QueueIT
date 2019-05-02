using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Models
{
    [Table("UserNotificationSettings")]
    public class UserNotificationSetting
    {
        public string UserId { get; set; }
        public bool NotificationsOn { get; set; }
    }
}