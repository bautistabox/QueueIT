using System.Collections.Generic;
using QueueIT.Controllers.Account;

namespace QueueIT.Models
{
    public class QueueShowViewModel
    {
        public string CurrentUserId { get; set; }
        public Queue CurrentQueue { get; set; }
        public Team CurrentTeam { get; set; }
        public List<Task> TaskList { get; set; } 
       }
}