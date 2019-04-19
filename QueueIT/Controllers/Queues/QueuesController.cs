using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueueIT.Identity;
using QueueIT.Models;

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
                var team = _db.Teams.FirstOrDefault(t => t.Id == queue.TeamId);
                if (team != null)
                {
                    var model = new QueueShowViewModel
                    {
                        CurrentQueue = queue,
                        CurrentTeam = team
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
        public IActionResult CreateTask(string testInput)
        {
            Console.WriteLine("In CREATETASK METHOD ACTION");
            Console.WriteLine("testInput");
            return RedirectToAction("UserHome", "Account");
        }
    }
}