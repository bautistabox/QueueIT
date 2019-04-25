using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Models
{
    [Table("Tasks")]
    public class Task
    {
        public int Id { get; set; }
        public int QueueId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatorId { get; set; }
        public int AssigneeId { get; set; }
        public int Status { get; set; }
        public DateTime? DueOn { get; set; }
    }
}