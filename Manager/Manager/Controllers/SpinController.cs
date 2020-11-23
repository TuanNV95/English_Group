using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    public class SpinController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
