using Manager.Common;
using Manager.Connection;
using Manager.Models.TableModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manager.Controllers
{
    public class ManagerController : _DbExcute
    {
        public IActionResult Index()
        {
            ViewBag.Session = HttpContext.Session.GetString(Constants.MENU_ACTIVE);
            ViewBag.Role = HttpContext.Session.GetString(Constants.ROLE);
            ViewBag.NameFacebook = HttpContext.Session.GetString(Constants.NAME_FACEBOOK);
            return View();
        }

        [HttpGet]
        public PartialViewResult ListUser()
        {
            try
            {
                if (HttpContext.Session.GetString(Constants.ROLE) == Constants.ADMIN)
                {
                    var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                        .OrderByDesc(ConstantsDatabase.USERS_ACTIVED)
                        .Get<user>();
                    return PartialView("UserListPartialView", data.ToList());
                }
                else
                    return PartialView("UserListPartialView", new List<user>());
            }
            catch
            {
                return PartialView("UserListPartialView", new List<user>());
            }
        }

        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                var query_delete = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.COL_ID, id);
                var user = query_delete.Clone().FirstOrDefault<user>();
                if (!(user is null) && user.actived == 100)
                    return Json(new { ErrorCode = 400, Message = "error!" });
                var stt_delete = query_delete.Clone().Delete();
                if (stt_delete > 0)
                    return Json(new { ErrorCode = 200, Message = "sucsess!" });
                return Json(new { ErrorCode = 400, Message = "error!" });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ActiveUser(int id)
        {
            try
            {
                var stt_active = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.COL_ID, id)
                    .Update(new
                    {
                        actived = 1
                    });
                if (stt_active > 0)
                    return Json(new { ErrorCode = 200, Message = "sucsess!" });
                return Json(new { ErrorCode = 400, Message = "error!" });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message });
            }
        }
    }
}
