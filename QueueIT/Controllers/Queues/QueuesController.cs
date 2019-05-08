using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueueIT.Identity;
using QueueIT.Models;
using QueueIT.Notifications;
using Task = QueueIT.Models.Task;

namespace QueueIT.Controllers.Queues
{
    public class QueuesController : Controller
    {
        private readonly UserManager<QueueItUser> _userManager;
        private readonly QueueItDbContext _db;
        private readonly QueueItUserDbContext _userDb;

        public QueuesController(UserManager<QueueItUser> userManager, QueueItDbContext db, QueueItUserDbContext userDb)
        {
            _userManager = userManager;
            _db = db;
            _userDb = userDb;
        }

        [HttpPost]
        public IActionResult QueueUpdate(QueueUpdateInputModel model)
        {
            var queue = _db.Queues.FirstOrDefault(q => q.Id == model.QueueId);
            if (queue == null) return RedirectToAction("Show", new {queueId = model.QueueId});
            queue.Title = model.QueueTitle;
            queue.IsPrivate = model.QueueVisibility.Equals("private");
            _db.SaveChanges();

            return RedirectToAction("Show", new {queueId = model.QueueId});
        }
        
        [HttpGet]
        [Route("queues/show/{queueId}")]
        public IActionResult Show(int queueId)
        {
            var queue = _db.Queues.FirstOrDefault(q => q.Id == queueId);

            if (queue != null)
            {
                var tasks = _db.Tasks.Where(t => t.QueueId == queueId).ToList();

                var team = _db.Teams.FirstOrDefault(t => t.Id == queue.TeamId);
                if (team != null)
                {
                    var model = new QueueShowViewModel
                    {
                        CurrentUserId = _userManager.GetUserId(HttpContext.User),
                        CurrentQueue = queue,
                        CurrentTeam = team,
                        TaskList = tasks
                    };

                    return View(model);
                }
            }

            var errModel = new QueueShowViewModel
            {
                CurrentQueue = new Models.Queue
                {
                    Title = "Queue Not Found",
                    IsPrivate = true
                },
                CurrentTeam = new Team
                {
                    Name = "Team Not Found"
                }
            };
            return View(errModel);
        }

        [HttpPost]
        public IActionResult CreateQueue(string queueTitle, int queueTeam, string queueVisibility)
        {
            var isPrivate = (queueVisibility.Equals("private"));

            if (string.IsNullOrEmpty(queueTitle) || (queueTitle == "Personal" ||  queueTitle == "Personal Queue")) return RedirectToAction("UserHome", "Account");
            
            var queue = new Queue
            {
                Title = queueTitle,
                TeamId = queueTeam,
                CreatorId = _userManager.GetUserId(HttpContext.User),
                IsPrivate = isPrivate,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
            };

            _db.Queues.Add(queue);
            _db.SaveChanges();

            return RedirectToAction("UserHome", "Account");
        }

        [HttpPost]
        public void DeleteQueue([FromBody] DeleteQueueInputModel model)
        {
            var queue = _db.Queues.FirstOrDefault(q => q.Id == model.QId);
            if (queue == null || queue.Title == "Personal Queue") return;
            
            
            
            var tasks = _db.Tasks.Where(t => t.QueueId == model.QId).ToList();
            
            foreach (var task in tasks)
            {
                _db.Tasks.Remove(task);
            }

            _db.Queues.Remove(queue);

            _db.SaveChanges();
        }

        [HttpPost]
        public int CreateTask([FromBody] TaskInputModel model)
        {
            var existingTasksInCurrentQ = _db.Tasks.Where(t => t.QueueId == model.QueueId).ToList();
            foreach (var existingTask in existingTasksInCurrentQ)
            {
                if (existingTask.Title == model.TaskTitle)
                {
                    return -1;
                }
            }

            var task = new Task
            {
                QueueId = model.QueueId,
                Title = model.TaskTitle,
                CreatorId = model.CreatorId,
                Status = model.Status
            };
            _db.Tasks.Add(task);
            _db.SaveChanges();

            return task.Id;
        }

        [HttpPost]
        public bool UpdateTaskStatus([FromBody] TaskUpdateInputModel model)
        {
            Console.WriteLine("in updateTaskStatus");
            var task = _db.Tasks.FirstOrDefault(t => t.Id == model.TaskId);

            if (task != null) task.Status = model.NewTaskStatus;

            _db.SaveChanges();

            return true;
        }

        [HttpGet]
        public ActionResult<TaskDetailsModalViewModel> DetailModal(int taskId)
        {
            
            var task = _db.Tasks.FirstOrDefault(t => t.Id == taskId);
            var taskUser = _userDb.Users.FirstOrDefault(u => u.Id == task.AssigneeId);
            var queue = _db.Queues.FirstOrDefault(q => q.Id == task.QueueId);
            var team = _db.Teams.FirstOrDefault(t => t.Id == queue.TeamId);
            var userTeams = _db.UserTeams.Where(ut => ut.TeamId == team.Id).ToList();
            var teamMembers = new List<QueueItUser>();
            
            foreach (var user in userTeams)
            {
                teamMembers.Add(_userDb.Users.FirstOrDefault(u => u.Id == user.UserId));
            }
            
            var model = new TaskDetailsModalViewModel
            {
                TheTask = task,
                TeamMembers = teamMembers
            };

            if (taskUser == null) return model;
            
            model.UserName = taskUser.UserName;
            model.Name = taskUser.FirstName + " " + taskUser.LastName;
            model.Initials = taskUser.FirstName.Substring(0, 1) + taskUser.LastName.Substring(0, 1);
            model.UserId = taskUser.Id;

            return model;
        }

        [HttpPost]
        public int DetailModal([FromBody] TaskDetailInputModel model)
        {
            var task = _db.Tasks.FirstOrDefault(t => t.Id == model.TaskId);
            if (task == null)
            {
                return -1;
            }

            task.Title = model.NewTaskTitle;
            task.Description = model.NewTaskDesc;
            task.DueOn = model.NewTaskDueOn;

            _db.SaveChanges();

            return model.TaskId;
        }

        [HttpPost]
        public IActionResult DeleteTask(int taskIdDel, int queueIdDel)
        {
            var task = _db.Tasks.FirstOrDefault(t => t.Id == taskIdDel);
            if (task == null) return RedirectToAction("Show", new {queueId = queueIdDel});
            _db.Tasks.Remove(task);
            _db.SaveChanges();
            return RedirectToAction("Show", new {queueId = queueIdDel});
        }

        [HttpPost]
        public ActionResult<Task> AssignUserToTask([FromBody] AssignUserToTaskInputModel model)
        {
            var task = _db.Tasks.FirstOrDefault(t => t.Id == model.TaskId);
            if (task == null) return new Task();
            
            task.AssigneeId = model.UserId;
            _db.SaveChanges();
                
            return task;
        }

        [HttpPost]
        public void SendAssignedTaskNotification([FromBody] Task model)
        {
            var user = _userDb.Users.FirstOrDefault(u => u.Id == model.AssigneeId);

            if (user == null) return;
            
            var notification = new Notification
            {
                Message = "You have been assigned to: " + model.Title,
                Link = "/Queues/Show/" + model.QueueId,
                IsRead = false,
                ToId = model.AssigneeId,
                ToName = user.FirstName + user.LastName,
                Type = 3,
                CreatedAt = DateTime.Now,
                
            };

            _db.Notifications.Add(notification);
            _db.SaveChanges();
        }
    }
}