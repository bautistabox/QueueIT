namespace QueueIT.Models
{
    public class TaskInputModel
    {
        public string TaskTitle { get; set; }
        public int QueueId { get; set; }
        public string CreatorId { get; set; }
        public int Status { get; set; }
    }
}