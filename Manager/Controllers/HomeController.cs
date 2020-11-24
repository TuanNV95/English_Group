using Manager.Connection;
using Manager.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Manager.Controllers
{
    public class HomeController : _DbExcute
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var id_facebook = HttpContext.Session.GetString("IdFacebook");
            return View();
        }

        [HttpGet]
        public ActionResult SelectMenu(string id)
        {
            var action = id;
            switch(id)
            {
                case "home":
                    SesionHelper.menu_active = 1;
                    break;
                case "spin":
                    SesionHelper.menu_active = 2;
                    break;
                case "punish":
                    SesionHelper.menu_active = 3;
                    break;
                case "notification":
                    SesionHelper.menu_active = 4;
                    break;
                case "profile":
                case "_profile":
                    action = "profile";
                    SesionHelper.menu_active = 5;
                    break;
                case "friend":
                    SesionHelper.menu_active = 6;
                    break;
                case "setting":
                    SesionHelper.menu_active = 7;
                    break;
                default:
                    action = null;
                    break;
            }
            return Json(new { Message = "success!", id = action });
        }
    }
}
