using Manager.Common;
using Manager.Connection;
using Manager.Helper;
using Manager.Models.TableModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manager.Controllers
{
    public class FriendController : _DbExcute
    {
        public IActionResult Index()
        {
            // TODO: Dùng tạm do chưa có chức năng login
            if (HttpContext.Session is null || String.IsNullOrEmpty(HttpContext.Session.GetString(Constants.ID_FACEBOOK)))
                return SesionHelper.CheckLogin();

            ViewBag.Session = HttpContext.Session.GetString(Constants.MENU_ACTIVE);
            ViewBag.Role = HttpContext.Session.GetString(Constants.ROLE);
            ViewBag.NameFacebook = HttpContext.Session.GetString(Constants.NAME_FACEBOOK);
            return View();
        }

        [HttpGet]
        public PartialViewResult ListFriend()
        {
            try
            {
                string current_id_fb = ViewBag.NameFacebook = HttpContext.Session.GetString(Constants.ID_FACEBOOK);
                var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                        .Where(ConstantsDatabase.USERS_ID_FACEBOOK, "!=", current_id_fb)
                        .Get<user>();
                return PartialView("AllFriendsPartialView", data.ToList());
            }
            catch
            {
                return PartialView("AllFriendsPartialView", new List<user>());
            }
        }

        [HttpGet]
        public ActionResult DetailsUser(int id)
        {
            try
            {
                string current_id_fb = ViewBag.NameFacebook = HttpContext.Session.GetString(Constants.ID_FACEBOOK);
                var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                        .Where(ConstantsDatabase.COL_ID, id)
                        .FirstOrDefault<user>();
                if (!(data is null))
                    return Json(new { ErrorCode = 200, Message = "success!", data = data });
                return Json(new { ErrorCode = 400, Message = "error!", data = new user() });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message, data = new user() });
            }
        }
    }
}
