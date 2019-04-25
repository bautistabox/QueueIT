using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Models
{
    [Table("UserTeams")]
    public class UserTeam
    {
        public string UserId { get; set; }
        public int TeamId { get; set; }
        public bool IsAdmin { get; set; }
    }
}