using System.Collections.Generic;
using QueueIT.Notifications;

namespace QueueIT.Models
{
    public class LayoutViewModel
    {
        public List<Queue> Queues { get; set; }
        public List<Team> Teams { get; set; }
        public List<Notification> Notifications { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserInitials { get; set; }
        public string UserName { get; set; }   
    }
}