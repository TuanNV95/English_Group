using Manager.Common;
using Manager.Connection;
using Manager.Helper;
using Manager.Models;
using Manager.Models.TableModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

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
            // TODO: Dùng tạm do chưa có chức năng login
            if (HttpContext.Session is null || String.IsNullOrEmpty(HttpContext.Session.GetString(Constants.ID_FACEBOOK)))
                return SesionHelper.CheckLogin();


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

        [HttpGet]
        public PartialViewResult GetDataTask()
        {
            try
            {
                var date_week = Utilities.GetAllDayInWeek();
                var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS + " as ut")
                    .Join(ConstantsDatabase.TABLE_USERS + " as u", "ut.user_id", "u.id")
                    .Where("ut.date", ">=", date_week[0])
                    .Where("ut.date", "<", date_week[6].AddDays(1))
                    .Select("ut.*", "u.id_facebook", "u.full_name")
                    .OrderBy("ut.date")
                    .Get<user_task_response>().ToList();
                foreach (var item in data)
                {
                    var day = item.date.GetValueOrDefault().DayOfWeek.ToString();
                    switch (day)
                    {
                        case "Monday":
                            item.task = "Thử thách thứ " + 2;
                            break;
                        case "Tuesday":
                            item.task = "Thử thách thứ " + 3;
                            break;
                        case "Wednesday":
                            item.task = "Thử thách thứ " + 4;
                            break;
                        case "Thursday":
                            item.task = "Thử thách thứ " + 5;
                            break;
                        case "Friday":
                            item.task = "Thử thách thứ " + 6;
                            break;
                        case "Saturday":
                            item.task = "Thử thách thứ " + 7;
                            break;
                        case "Sunday":
                            item.task = "Tạo chủ đề tuần";
                            break;
                    }
                }
                return PartialView("TaskPartialView", data);
            }
            catch
            {
                return PartialView("TaskPartialView", new List<user_task_response>());
            }
        }


        [HttpGet]
        public PartialViewResult GetDataPunish()
        {
            try
            {
                var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS + " as up")
                    .Join(ConstantsDatabase.TABLE_USERS + " as u", "up.user_id", "u.id")
                    .Where("up.status", 0)
                    .Select("up.*", "u.id_facebook", "u.full_name")
                    .OrderByDesc("up.date_created")
                    .Get<user_punishs_response>().ToList();
                var lst_user_id = data.Select(c => c.user_id);
                var lst_sum = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS)
                    .WhereIn("user_id", lst_user_id).Get<user_punishs>().ToList();
                foreach (var item in data)
                {
                    item.sum = lst_sum.Where(c => c.user_id == item.user_id).Count();
                }
                return PartialView("PunishLogPartialView", data);
            }
            catch
            {
                return PartialView("PunishLogPartialView", new List<user_punishs_response>());
            }
        }

        [HttpGet]
        public ActionResult GetSumDataPunish()
        {
            try
            {
                var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS)
                    .Where("status", 0).AsCount().First<int>();
                return Json(new { ErrorCode = 200, Message = "success!", sum = data });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message, sum = -1 });
            }
        }

        [HttpGet]
        public PartialViewResult GetRolePunish()
        {
            try
            {
                var data = new spin_role();
                var today = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
                var check_data = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS + " as ut")
                    .Join(ConstantsDatabase.TABLE_USERS + " as u", "u.id", "ut.user_id")
                    .Where("ut.date", ">=", today.AddDays(-1))
                    .Where("ut.date", "<", today)
                    .Select("u.*")
                    .FirstOrDefault<user>();
                var id_fb = HttpContext.Session.GetString(Constants.ID_FACEBOOK);
                var check_admin = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.USERS_ID_FACEBOOK, id_fb)
                    .FirstOrDefault<user>();
                data.is_admin = check_admin.actived == 100 ? true : false;
                if (check_data.id == check_admin.id)
                {
                    data.is_spiner = true;
                    data.full_name = check_data.full_name;
                }
                else
                {
                    data.is_spiner = false;
                    data.full_name = check_data.full_name;
                }
                return PartialView("RolePunishPartialView", data);
            }
            catch
            {
                return PartialView("RolePunishPartialView", new spin_role());
            }
        }

        [HttpGet]
        public PartialViewResult GetAllUserActive()
        {
            try
            {
                var today = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
                var _sup_query = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS)
                    .Where(ConstantsDatabase.USER_PUNISHS_DATE_CREATED, ">=", today.AddDays(-1))
                    .Where(ConstantsDatabase.USER_PUNISHS_DATE_CREATED, "<", today);

                var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS + " as u")
                    .LeftJoin(_sup_query.As("up"), j => j.On("up.user_id", "u.id"))
                    .Where("u.status", 1)
                    .Select("u.*", "up.id as punish_id")
                    .Get<user_punish>().ToList();
                return PartialView("AllUserActivePartialView", data);
            }
            catch (Exception ex)
            {
                return PartialView("AllUserActivePartialView", new List<user_punish>());
            }
        }

        [HttpPost]
        public ActionResult UpdatePunish(List<int> lst_id)
        {
            try
            {
                var stt = -1;
                var today = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
                stt = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS)
                .Where(ConstantsDatabase.USER_PUNISHS_DATE_CREATED, ">=", today.AddDays(-1))
                .Where(ConstantsDatabase.USER_PUNISHS_DATE_CREATED, "<", today)
                .Delete();
                foreach (var item in lst_id)
                {
                    var id_fb = HttpContext.Session.GetString(Constants.ID_FACEBOOK);
                    var this_user = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.USERS_ID_FACEBOOK, id_fb)
                    .FirstOrDefault<user>();
                    stt = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS)
                        .InsertGetId<int>(new
                        {
                            user_id = item,
                            punish_contents = "Bài tập của " + this_user.full_name + " ngày " + today.AddDays(-1).ToString("MM/dd/yyyy"),
                            date_created = today.AddDays(-1),
                            user_created = this_user.id,
                            status = 0
                        });
                }
                if (stt > 0)
                    return Json(new { ErrorCode = 200, Message = "success!" });
                return Json(new { ErrorCode = 400, Message = "error!" });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message });
            }
        }
    }
}
