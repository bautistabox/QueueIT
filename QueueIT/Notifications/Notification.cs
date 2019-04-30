using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueIT.Notifications
{
    [Table("Notifications")]
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string ToId { get; set; }
        public string ToName { get; set; }
        public string FromName { get; set; }
        public string FromId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string Link { get; set; }
        public int ToTeamId { get; set; }
        public int Type { get; set; }
    }
    
    
}