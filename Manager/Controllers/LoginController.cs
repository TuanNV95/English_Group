using Manager.Connection;
using Manager.Helper;
using Manager.Models.TableModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SqlKata.Execution;

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

        [HttpPost]
        public ActionResult LoginWithFacebook(string id, string name)
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
                        user_name = name
                    });
            }
            else
            {
                new_user = 1;
            }
            if (insert_status > 0)
            {
                HttpContext.Session.SetString("IdFacebook", id);
                var user_selected = query_data.Clone().FirstOrDefault<user>();
                return Json(new { ErrorCode = 200, Message = "success!", NewUser = new_user, DataUser = user_selected });
            }
            return Json(new { ErrorCode = 400, Message = "error!" });
        }

        public IActionResult Facebook()
        {
            ViewBag.IdFacebook = HttpContext.Session.GetString("IdFacebook");
            return View();
        }

        [HttpPost]
        public ActionResult ContinueWithFacebook(string email, string id_facebook)
        {
            var query_data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                .Where(ConstantsDatabase.USERS_ID_FACEBOOK, id_facebook);
            if (query_data.Clone().AsCount().First<int>() > 0)
            {
                var update_status = query_data.Clone()
                    .Update(new
                    {
                        email = email
                    });
                if (update_status > 0)
                {
                    var user_selected = query_data.Clone().FirstOrDefault<user>();
                    return Json(new { ErrorCode = 200, Message = "success!", DataUser = user_selected });
                }
            }
            return Json(new { ErrorCode = 400, Message = "error!" });
        }

    }
}
