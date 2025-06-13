using Microsoft.AspNetCore.Mvc;

namespace serverSKUD.Controllers
{
    public class LogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
