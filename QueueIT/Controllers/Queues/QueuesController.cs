using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QueueIT.Identity;
using QueueIT.Models;
using Task = QueueIT.Models.Task;

namespace QueueIT.Controllers.Queue
{
    public class QueuesController : Controller
    {
        private readonly UserManager<QueueItUser> _userManager;
        private readonly QueueItDbContext _db;

        public QueuesController(UserManager<QueueItUser> userManager, QueueItDbContext db)
        {
            _userManager = userManager;
            _db = db;
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

            var queue = new Models.Queue
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
        public ActionResult<Task> DetailModal(int taskId)
        {
            var task = _db.Tasks.FirstOrDefault(t => t.Id == taskId);
            return task;
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
    }
}