using System.Collections.Generic;
using QueueIT.Identity;

namespace QueueIT.Models
{
    public class TaskDetailsModalViewModel
    {
        public Task TheTask { get; set; }
        public List<QueueItUser> TeamMembers { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string UserId { get; set; }
    }
}