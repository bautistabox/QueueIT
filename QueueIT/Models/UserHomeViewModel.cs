using System.Collections.Generic;

namespace QueueIT.Models
{
    public class UserHomeViewModel
    {
        public List<Team> TeamsList { get; set; }
        public List<Models.Queue> QueuesList { get; set; }
    }
}