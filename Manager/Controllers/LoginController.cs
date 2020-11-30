using Manager.Common;
using Manager.Connection;
using Manager.Helper;
using Manager.Models.TableModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SqlKata.Execution;
using System;
using System.Linq;

namespace Manager.Controllers
{
    public class LoginController : _DbExcute
    {
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config) : base()
        {
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Login/LoginWithFacebook")]
        public ActionResult LoginWithFacebook(string id, string name)
        {
            try
            {
                var new_user = 0;
                var query_data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.USERS_ID_FACEBOOK, id);
                var insert_status = 1;
                if (query_data.Clone().AsCount().First<int>() <= 0)
                {
                    insert_status = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                        .InsertGetId<int>(new
                        {
                            id_facebook = id,
                            full_name = name,
                            actived = 0,
                            status = 1,
                            spined = 0,
                            is_spiner = 0
                        });
                }
                var user_selected = query_data.Clone().FirstOrDefault<user>();
                if (String.IsNullOrEmpty(user_selected.email))
                {
                    new_user = 1;
                }
                if (insert_status > 0)
                {
                    HttpContext.Session.SetString(Constants.ID_FACEBOOK, id);
                    HttpContext.Session.SetString(Constants.NAME_FACEBOOK, name);
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.HOME);
                    if (user_selected.actived == 100)
                        HttpContext.Session.SetString(Constants.ROLE, Constants.ADMIN);
                    else if (user_selected.actived == 0)
                    {
                        new_user = 2;
                        HttpContext.Session.SetString(Constants.ROLE, Constants.NEW);
                    }
                    return Json(new { ErrorCode = 200, Message = "success!", NewUser = new_user });
                }
                return Json(new { ErrorCode = 400, Message = "error!" });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message });
            }
        }

        public IActionResult Facebook()
        {
            // TODO: Dùng tạm do chưa có chức năng login
            if (HttpContext.Session is null || String.IsNullOrEmpty(HttpContext.Session.GetString(Constants.ID_FACEBOOK)))
                return SesionHelper.CheckLogin();

            ViewBag.IdFacebook = HttpContext.Session.GetString(Constants.ID_FACEBOOK);
            ViewBag.NameFacebook = HttpContext.Session.GetString(Constants.NAME_FACEBOOK);
            ViewBag.Role = HttpContext.Session.GetString(Constants.ROLE);
            return View();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult ContinueWithFacebook(string email, string id_facebook)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(id_facebook))
            {
                return Json(new { ErrorCode = 404, Message = "error!" });
            }
            if (!IsValidEmail(email))
            {
                return Json(new { ErrorCode = 405, Message = "error!" });
            }
            var query_data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                .Where(ConstantsDatabase.USERS_ID_FACEBOOK, id_facebook);
            var user_selected = query_data.Clone().FirstOrDefault<user>();
            if (!(user_selected is null))
            {
                if (!String.IsNullOrEmpty(user_selected.email))
                {
                    return Json(new { ErrorCode = 406, Message = "error!" });
                }
                var update_status = query_data.Clone()
                    .Update(new
                    {
                        email = email
                    });
                if (update_status > 0)
                {
                    return Json(new { ErrorCode = 200, Message = "success!", DataUser = user_selected });
                }
            }
            return Json(new { ErrorCode = 400, Message = "error!" });
        }

        [HttpPost]
        public ActionResult LoginByEmail(string email, string pass)
        {
            try
            {
                var hash_pass = Utilities.Sha256Hash(pass);
                var user = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(new
                    {
                        email = email,
                        password = hash_pass
                    })
                    .FirstOrDefault<user>();
                if (!(user is null))
                {
                    HttpContext.Session.SetString(Constants.ID_FACEBOOK, user.id_facebook);
                    HttpContext.Session.SetString(Constants.NAME_FACEBOOK, user.full_name);
                    HttpContext.Session.SetString(Constants.MENU_ACTIVE, Constants.HOME);
                    if (user.actived == 100)
                        HttpContext.Session.SetString(Constants.ROLE, Constants.ADMIN);
                    else if (user.actived == 0)
                    {
                        HttpContext.Session.SetString(Constants.ROLE, Constants.NEW);
                    }
                    return Json(new { ErrorCode = 200, Message = "success!" });
                }
                return Json(new { ErrorCode = 400, Message = "error!" });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message });
            }
        }
    }
}
