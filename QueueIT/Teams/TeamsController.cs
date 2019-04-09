using System;
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
            _userDb= _qudb;
            _userManager = _um;
        }
        
        [HttpPost]
        public IActionResult CreateTeam(string teamName, string desc)
        {      
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
            
    }
}