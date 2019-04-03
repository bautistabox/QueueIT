using Microsoft.AspNetCore.Mvc;

namespace QueueIT.Controllers.Queue
{
    public class QueuesController : Controller
    {
        public IActionResult Show()
        {
            return View();
        }
    }
}