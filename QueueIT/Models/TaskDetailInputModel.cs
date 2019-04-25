using System;

namespace QueueIT.Models
{
    public class TaskDetailInputModel
    {
        public int TaskId { get; set; }
        public string NewTaskTitle { get; set; }
        public string NewTaskDesc { get; set; }
        public DateTime? NewTaskDueOn { get; set; }
    }
}