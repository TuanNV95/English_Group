using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
