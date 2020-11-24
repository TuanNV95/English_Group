using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    public class FriendController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
