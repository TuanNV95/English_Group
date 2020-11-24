using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
