using System.Collections.Generic;
using QueueIT.Identity;

namespace QueueIT.Models
{
    public class TeamProfileViewModel
    {
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public string TeamCreatorId { get; set; }
        public bool IsPrivate { get; set; }
        public List<Queue> Queues { get; set; }
        public List<QueueItUserWithRole> TeamMembers { get; set; }
        
    }

    public class QueueItUserWithRole
    {
        public QueueItUser User { get; set; }
        public bool IsAdmin { get; set; }
    }
}