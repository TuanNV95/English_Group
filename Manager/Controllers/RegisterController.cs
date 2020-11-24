using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
