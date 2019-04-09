using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Models
{
    [Table("Teams")]
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}