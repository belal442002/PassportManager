using Microsoft.AspNetCore.Mvc;

namespace Document_Intelligence_Task.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
