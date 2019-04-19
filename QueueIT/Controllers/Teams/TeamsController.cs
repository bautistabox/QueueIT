using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.UrlMatches;
using QueueIT.Identity;
using QueueIT.Models;

namespace QueueIT.Teams
{
    public class TeamsController : Controller
    {
        private readonly QueueItDbContext _db;
        private readonly QueueItUserDbContext _userDb;
        private readonly UserManager<QueueItUser> _userManager;

        public TeamsController(QueueItDbContext _qdb, QueueItUserDbContext _qudb, UserManager<QueueItUser> _um)
        {
            _db = _qdb;
            _userDb = _qudb;
            _userManager = _um;
        }

        [HttpPost]
        public IActionResult CreateTeam(string teamName, string desc)
        {
            var normalizedTeamName = teamName.ToUpper();
            if (normalizedTeamName.Equals("PERSONAL"))
            {
                Console.WriteLine("TEAMNAME CANNOT BE \'PERSONAL\'");
                return RedirectToAction("UserHome", "Account");
            }
            
            var currentUserId = _userManager.GetUserId(HttpContext.User);

            var teams = _db.Teams.Where(t => t.CreatorId == currentUserId).ToList();

            foreach (var t in teams)
            {
                var normalizedTName = t.Name.ToUpper();
                if (!normalizedTName.Equals(normalizedTeamName)) continue;
                Console.WriteLine("Team Already Exists");
                return RedirectToAction("UserHome", "Account");
            }
            
            var team = new Team
            {
                Name = teamName,
                CreatorId = _userManager.GetUserId(HttpContext.User),
                Description = desc,
                IsPrivate = true,
                CreatedOn = DateTime.Now
            };

            _db.Teams.Add(team);
            _db.SaveChanges();

            return RedirectToAction("UserHome", "Account");
        }

        [HttpGet]
        [Route("/teams/show/{teamId}")]
        public IActionResult Show(int teamId)
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User);

            var team = _db.Teams.FirstOrDefault(t => t.Id == teamId);
            var queues = _db.Queues.Where(q => q.TeamId == teamId).ToList();
            if (team == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new TeamShowViewModel
            {
                CurrentUserId = currentUserId,
                TeamId = team.Id,
                TeamName = team.Name,
                TeamCreatorId = team.CreatorId,
                Description = team.Description,
                IsPrivate = team.IsPrivate,
                QueuesList = queues
            };
                
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTeam(int teamId)
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            var team = _db.Teams.FirstOrDefault(t => t.Id == teamId);
            
            if (team == null)
            {
                Console.WriteLine("NO TEAM FOUND");
                return RedirectToAction("UserHome", "Account");
            }

            var queues = _db.Queues.Where(q => q.TeamId == teamId).ToList();
            
            if (team.CreatorId != currentUserId)
            {
                Console.WriteLine("NO PERMISSION TO DELETE TEAM");
                return RedirectToAction("UserHome", "Account");
            }

            if (team.Name.Equals("Personal"))
            {
                Console.WriteLine("CANNOT DELETE PERSONAL TEAM");
                return RedirectToAction("UserHome", "Account");
            }

            foreach (var queue in queues)
            {
                _db.Queues.Remove(queue);
            }
            _db.Teams.Remove(team);
            _db.SaveChanges();
            return RedirectToAction("UserHome", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePrivacy(int teamId)
        {
            Console.WriteLine("In ChangePrivacy");
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            var team = _db.Teams.FirstOrDefault(t => t.Id == teamId);

            if (team == null)
            {
                Console.WriteLine("NO TEAM FOUND");
                return RedirectToAction("UserHome", "Account");
            }
            if (team.CreatorId != currentUserId)
            {
                Console.WriteLine("NO PERMISSION TO DELETE TEAM");
                return RedirectToAction("UserHome", "Account");
            }
            
            var queues = _db.Queues.Where(q => q.TeamId == teamId).ToList();
            
            var model = new TeamShowViewModel
            {
                CurrentUserId = currentUserId,
                TeamId = team.Id,
                TeamName = team.Name,
                TeamCreatorId = team.CreatorId,
                Description = team.Description,
                IsPrivate = team.IsPrivate,
                QueuesList = queues
            };
            

            team.IsPrivate = !team.IsPrivate;
            _db.SaveChanges();
            
            return RedirectToAction("Show", model);            
        }
    }
}