using System.Collections.Generic;

namespace QueueIT.Models
{
    public class TeamShowViewModel
    {
        public string CurrentUserId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamCreatorId { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public List<Queue> QueuesList { get; set; }
        
    }
}