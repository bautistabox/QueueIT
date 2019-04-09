using System.Collections.Generic;
using QueueIT.Models;

namespace QueueIT.Controllers.Account
{
    public class UserHomeViewModel
    {
        public List<Team> UserTeams { get; set; }
        public List<Models.Queue> UserQueues { get; set; }
    }
}