using Manager.Common;
using Manager.Connection;
using Manager.Models.TableModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System;

namespace Manager.Controllers
{
    public class ProfileController : _DbExcute
    {
        public IActionResult Index()
        {
            var your_id_fb = HttpContext.Session.GetString(Constants.ID_FACEBOOK);
            ViewBag.Session = HttpContext.Session.GetString(Constants.MENU_ACTIVE);
            ViewBag.Role = HttpContext.Session.GetString(Constants.ROLE);
            ViewBag.IdFacebook = your_id_fb;
            ViewBag.NameFacebook = HttpContext.Session.GetString(Constants.NAME_FACEBOOK);
            var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                        .Where(ConstantsDatabase.USERS_ID_FACEBOOK, your_id_fb)
                        .FirstOrDefault<user>();
            return View(data == null ? new user() : data);
        }

        [HttpPost]
        public ActionResult UpdateUser(user user)
        {
            try
            {
                var stt_update = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.COL_ID, user.id)
                    .Update(new
                    {
                        user_name = user.user_name,
                        password = user.password,
                        email = user.email,
                        facebook = user.facebook,
                        instagram = user.instagram,
                        twitter = user.twitter,
                        details = user.details
                    });
                if (stt_update > 0)
                    return Json(new { ErrorCode = 200, Message = "succsess!" });
                return Json(new { ErrorCode = 400, Message = "error!" });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message });
            }
        }
    }
}
