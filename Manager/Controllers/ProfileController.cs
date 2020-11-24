using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
