using System;
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
        
        public IActionResult Show()
        {
            
            
            return View();
        }

        [HttpPost]
        public IActionResult CreateQueue(string queueTitle, int queueTeam, string queueVisibility)
        {
            var isPrivate = (queueVisibility.Equals("Private"));
            
            var queue = new Models.Queue
            {
                Title = queueTitle,
                CreatorId = _userManager.GetUserId(HttpContext.User),
                IsPrivate = isPrivate,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
            };

            var teamQueue = new TeamQueue
            {
                TeamId = queueTeam,
                QueueId = queue.Id
            };

            _db.Queues.Add(queue);
            _db.TeamsQueues.Add(teamQueue);
            _db.SaveChanges();
            
            return RedirectToAction("UserHome", "Account");
        }
    }
}