using System.Collections.Generic;
using QueueIT.Models;

namespace QueueIT.Controllers.Account
{
    public class UserHomeViewModel
    {
        public List<Team> TeamsList { get; set; }
        public List<Models.Queue> QueuesList { get; set; }
    }
}