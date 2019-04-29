using System;
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
            var teams = _db.Teams.Where(t => t.CreatorId == currentUserId).ToList();
            var queues = _db.Queues.Where(q => q.CreatorId == currentUserId).ToList();
            
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
            var queues = new List<Models.Queue>();
            var teams = new List<Team>();

            if (userTeams.Count <= 0) return new LayoutViewModel
            {
                Queues = new List<Models.Queue>(),
                Teams = new List<Team>()
            };
            
            foreach (var userTeam in userTeams)
            {
                teams.Add(_db.Teams.FirstOrDefault(t => t.Id == userTeam.TeamId));
            }

            foreach (var team in teams)
            {
                queues.Add(_db.Queues.FirstOrDefault(q => q.TeamId == team.Id));
            }

            var user = _dbUser.Users.FirstOrDefault(u => u.Id == currentUserId);
            if(user == null) return new LayoutViewModel
            {
                Queues = new List<Models.Queue>(),
                Teams = new List<Team>()
            };
            
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
    }
}