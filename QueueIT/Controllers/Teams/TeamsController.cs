using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.UrlMatches;
using QueueIT.Identity;
using QueueIT.Models;
using QueueIT.Notifications;

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
            if (string.IsNullOrEmpty(teamName))
            {
                return RedirectToAction("UserHome", "Account");
            }
            
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

            if (desc == null)
            {
                desc = teamName + "'s description.";
                
            }

            var team = new Team
            {
                Name = teamName,
                CreatorId = currentUserId,
                Description = desc,
                IsPrivate = true,
                CreatedOn = DateTime.Now
            };
            _db.Teams.Add(team);
            _db.SaveChanges();
            
            var userTeam = new UserTeam
            {
                UserId = currentUserId,
                TeamId = team.Id,
                IsAdmin = true
            };
            _db.UserTeams.Add(userTeam);
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
            var teamMembers = _db.UserTeams.Where(ut => ut.TeamId == teamId).ToList();
            var teamMemberUsers = new List<QueueItUser>();
            foreach (var member in teamMembers)
            {
                teamMemberUsers.Add(_userDb.Users.FirstOrDefault(u => u.Id == member.UserId));
            }
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
                QueuesList = queues,
                TeamMembers = teamMembers,
                TeamMemberUsers = teamMemberUsers
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
            var userTeams = _db.UserTeams.Where(ut => ut.TeamId == teamId).ToList();

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
                var tasks = _db.Tasks.Where(t => t.QueueId == queue.Id).ToList();
                foreach (var task in tasks)
                {
                    _db.Tasks.Remove(task);
                }
                _db.Queues.Remove(queue);
            }

            foreach (var userTeam in userTeams)
            {
                _db.UserTeams.Remove(userTeam);
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

        [HttpPost]
        public IActionResult SaveProfile([FromForm] EditTeamProfileInputModel model)
        {
            var team = _db.Teams.FirstOrDefault(t => t.Id == model.TeamId);
            if (team != null)
            {
                if (model.NewTeamName != "" || model.NewTeamName != null)
                {
                    team.Name = model.NewTeamName;
                }

                if (string.IsNullOrEmpty(model.NewTeamDescription))
                {
                    model.NewTeamDescription = model.NewTeamName + "'s description.";
                }
                team.Description = model.NewTeamDescription;

                _db.SaveChanges();
            }

            return RedirectToAction("Show", new {teamId = model.TeamId});
        }

        [HttpPost]
        public bool CheckForUser([FromBody] CheckForUserInputModel model)
        {
            var user = _userDb.Users.FirstOrDefault(u => u.UserName == model.Username);
            return user != null;
        }

        [HttpPost]
        public void AddUserToTeam([FromBody] AddUserToTeamInputModel model)
        {
            var user = _userDb.Users.FirstOrDefault(u => u.UserName == model.Username);
            var currentUser = _userDb.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var team = _db.Teams.FirstOrDefault(t => t.Id == model.TeamId);

            if (user == null || currentUser == null) return;
            if (team == null) return;
            
            var message = "@" + currentUser.UserName + " added you to their team: " + team.Name + ".";
            var link = "/teams/show/" + team.Id;
            
            var notification = new Notification
            {   
                ToId = user.Id,
                FromId = currentUser.Id,
                ToName = user.UserName,
                FromName = currentUser.UserName,
                IsRead = false,
                Message = message,
                CreatedAt = DateTime.Now,
                Link = link,
                ToTeamId = team.Id,
                Type = 1
            };

            _db.Notifications.Add(notification);
            _db.SaveChanges();
        }

        [HttpPost]
        public void AcceptTeam([FromBody] AnswerTeamInputModel model)
        {
            var currentUser = _userDb.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var notification = _db.Notifications.FirstOrDefault(n => n.Id == model.NoteId);
            var team = _db.Teams.FirstOrDefault(t => t.Id == notification.ToTeamId);
            if (team == null) return;
            if (notification == null) return;
            if (currentUser == null) return;
            var userTeam = new UserTeam
            {
                TeamId = notification.ToTeamId,
                UserId = currentUser.Id,
                IsAdmin = false
            };
            _db.UserTeams.Add(userTeam);
            _db.SaveChanges();

            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            notification.Message = "You have accepted the invitation to join " + team.Name;
            _db.SaveChanges();
            var newNotification = new Notification
            {
                ToId = notification.FromId,
                ToName = notification.FromName,
                Message = "@" + notification.ToName + " has accepted the invitation to join your team.",
                FromId = currentUser.Id,
                FromName = currentUser.UserName,
                IsRead = false,
                ToTeamId = notification.ToTeamId,
                CreatedAt = DateTime.Now,
                Type = 2,
                Link = notification.Link
            };
            _db.Notifications.Add(newNotification);
            _db.SaveChanges();
        }

        [HttpPost]
        public void DeclineTeam([FromBody] AnswerTeamInputModel model)
        {
            var currentUser = _userDb.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var notification = _db.Notifications.FirstOrDefault(n => n.Id == model.NoteId);
            var team = _db.Teams.FirstOrDefault(t => t.Id == notification.ToTeamId);
            if (team == null) return;
            if (currentUser == null) return;
            if (notification == null) return;
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            notification.Message = "You have declined the invitation to join " + team.Name;
            _db.SaveChanges();
            var newNotification = new Notification
            {
                ToId = notification.FromId,
                ToName = notification.FromName,
                Message = "@" + notification.ToName + " has declined the invitation to join your team.",
                FromId = currentUser.Id,
                FromName = currentUser.UserName,
                IsRead = false,
                ToTeamId = notification.ToTeamId,
                CreatedAt = DateTime.Now,
                Type = 2
            };
            
            _db.Notifications.Add(newNotification);
            _db.SaveChanges();
        }

        [HttpPost]
        public void ReadNotification([FromBody] AnswerTeamInputModel model)
        {
            var notification = _db.Notifications.FirstOrDefault(n => n.Id == model.NoteId);
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            _db.SaveChanges();
        }
    }
}