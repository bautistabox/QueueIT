using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QueueIT.Identity;
using QueueIT.Models;

namespace QueueIT.Controllers.Users
{
    public class UsersController : Controller
    {
        private readonly QueueItDbContext _db;
        private readonly QueueItUserDbContext _userDb;
        private readonly UserManager<QueueItUser> _userManager;

        public UsersController(QueueItDbContext db, QueueItUserDbContext userDb, UserManager<QueueItUser> userManager)
        {
            _db = db;
            _userDb = userDb;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("users/profile/{userName}")]
        public IActionResult Profile(string userName)
        {
            var user = _userDb.Users.FirstOrDefault(u => u.UserName == userName);
            var userTeams = _db.UserTeams.Where(ut => ut.UserId == user.Id);
            var teams = new List<Team>();
            var queues = new List<Models.Queue>();
            foreach (var userTeam in userTeams)
            {
                teams.Add(_db.Teams.FirstOrDefault(t => t.Id == userTeam.TeamId));
            }

            foreach (var team in teams)
            {
                queues.Add(_db.Queues.FirstOrDefault(q => q.TeamId == team.Id));
            }

            if (user == null) return View();
            
            var model = new UserProfileViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserFullName = user.FirstName + " " + user.LastName,
                Teams = teams,
                Queues = queues
            };          
            
            return View(model);
        }
    }
}