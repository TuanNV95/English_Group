using Manager.Common;
using Manager.Connection;
using Manager.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Manager.Controllers
{
    //[Authorize]
    public class HomeController : _DbExcute
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger) : base()
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Session = HttpContext.Session.GetString(Constants.MENU_ACTIVE);
            ViewBag.Role = HttpContext.Session.GetString(Constants.ROLE);
            ViewBag.NameFacebook = HttpContext.Session.GetString(Constants.NAME_FACEBOOK);
            return View();
        }

        [HttpGet]
        public ActionResult SelectMenu(string id)
        {
            var action = id;
            switch (id)
            {
                case "home":
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.HOME);
                    break;
                case "spin":
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.SPIN);
                    break;
                case "punish":
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.PUNISH);
                    break;
                case "notification":
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.NOTIFICATION);
                    break;
                case "profile":
                case "_profile":
                    action = "profile";
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.PROFILE);
                    break;
                case "friend":
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.FRIEND);
                    break;
                case "setting":
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.SETTING);
                    break;
                case "manager":
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.MANAGER_ADMIN);
                    break;
                default:
                    action = null;
                    break;
            }
            return Json(new { Message = "success!", id = action });
        }
    }
}
