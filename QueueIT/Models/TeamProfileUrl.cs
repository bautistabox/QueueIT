using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Models
{
    [Table("TeamProfileUrls")]
    public class TeamProfileUrl
    {
        public int TeamId { get; set; }
        public string Url { get; set; }
    }
}