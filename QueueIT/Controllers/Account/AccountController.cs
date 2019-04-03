using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QueueIT.Controllers.Account
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult UserHome()
        {
            return View();
        }    
    }
}