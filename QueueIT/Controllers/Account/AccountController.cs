using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueueIT.Identity;
using QueueIT.Models;

namespace QueueIT.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly QueueItUserDbContext _dbUser;
        private readonly QueueItDbContext _db;
        private readonly UserManager<QueueItUser> _userManager;
          
        public AccountController(QueueItUserDbContext qiudb, QueueItDbContext qidb, UserManager<QueueItUser> um)
        {
            _dbUser = qiudb;
            _db = qidb;
            _userManager = um;
        }
        
        [HttpGet]
        public IActionResult UserHome()
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            var teams = _db.Teams.Where(t => t.CreatorId == currentUserId).ToList();
            var queues = _db.Queues.Where(q => q.CreatorId == currentUserId).ToList();

            var model = new UserHomeViewModel
            {
                UserTeams = teams,
                UserQueues = queues
            };
            
            return View(model);
        }    
    }
}