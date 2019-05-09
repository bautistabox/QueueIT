using System.Collections.Generic;
using System.Linq;
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
            var queues = new List<Queue>();
            var userTeams = _db.UserTeams.Where(ut => ut.UserId == currentUserId);

            var teams = new List<Team>();

            foreach (var userTeam in userTeams)
            {
                teams.Add(_db.Teams.FirstOrDefault(t => t.Id == userTeam.TeamId));
            }

            var teamsQueue = new List<Queue>();
            foreach (var team in teams)
            {
                teamsQueue = _db.Queues.Where(q => q.TeamId == team.Id).ToList();
                foreach (var teamQueue in teamsQueue)
                {
                    queues.Add(teamQueue);
                }
            }
            
            
            var model = new UserHomeViewModel
            {
                TeamsList = teams,
                QueuesList = queues
            };
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public ActionResult<LayoutViewModel> GetLayoutData()
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            var userTeams = _db.UserTeams.Where(ut => ut.UserId == currentUserId).ToList();
            var notifications = _db.Notifications.Where(n => n.ToId == currentUserId).ToList();
            var queues = new List<Queue>();
            var teams = new List<Team>();

            if (userTeams.Count <= 0) return new LayoutViewModel
            {
                Queues = new List<Queue>(),
                Teams = new List<Team>()
            };
            
            foreach (var userTeam in userTeams)
            {
                teams.Add(_db.Teams.FirstOrDefault(t => t.Id == userTeam.TeamId));
            }

            var teamQueues = new List<Queue>();
            foreach (var team in teams)
            {
                teamQueues = _db.Queues.Where(q => q.TeamId == team.Id).ToList();
                foreach (var teamQueue in teamQueues)
                {
                    if (teamQueue != null)
                    {
                        queues.Add(teamQueue);
                    }
                }
            }

            var user = _dbUser.Users.FirstOrDefault(u => u.Id == currentUserId);
            if(user == null) return new LayoutViewModel
            {
                Queues = new List<Models.Queue>(),
                Teams = new List<Team>()
            };

            notifications.Reverse();
                
            var model = new LayoutViewModel
            {
                Queues = queues,
                Teams = teams,
                Notifications = notifications,
                UserId = user.Id,
                UserEmail = user.Email,
                UserFirstName = user.FirstName,
                UserLastName = user.LastName,
                UserInitials = user.FirstName.Substring(0,1) + 
                               user.LastName.Substring(0,1),
                UserName = user.UserName
            };

            return model;
        }

        [HttpPost]
        public IActionResult SaveAccountDetails(SaveAccountDetailsInputModel model)
        {
            var user = _dbUser.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null) return RedirectToAction("Profile");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            
            if (_dbUser.Users.FirstOrDefault(u => u.UserName == model.Username) == null)
            {
                user.UserName = model.Username;
            }
            else
            {
                if (user.UserName == model.Username)
                {
                    _dbUser.SaveChanges();
                    return View("Profile");
                }
                
                _dbUser.SaveChanges();
                ModelState.AddModelError("uname-err", "Username already exists.");
                return View("Profile");
            }

        _dbUser.SaveChanges();
            
            return RedirectToAction("Profile");
        }
    }
}