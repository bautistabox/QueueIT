using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Models
{
    [Table("TeamsQueues")]
    public class TeamQueue
    {
        public int TeamId { get; set; }
        public int QueueId { get; set; }
    }
}