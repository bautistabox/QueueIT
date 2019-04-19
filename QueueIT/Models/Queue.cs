using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Models
{
    [Table("Queues")]
    public class Queue
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Title { get; set; }
        public string CreatorId { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}