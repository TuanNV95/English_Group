using Manager.Helper;
using Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
