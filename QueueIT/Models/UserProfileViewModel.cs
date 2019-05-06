using System.Collections.Generic;

namespace QueueIT.Models
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public List<Team> Teams { get; set; }
        public List<Queue> Queues { get; set; }
    }
}