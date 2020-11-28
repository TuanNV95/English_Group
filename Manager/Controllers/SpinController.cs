using Manager.Common;
using Manager.Connection;
using Manager.Helper;
using Manager.Models;
using Manager.Models.TableModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Manager.Controllers
{
    public class SpinController : _DbExcute
    {
        private readonly Random rand;
        public SpinController()
        {
            rand = new Random();
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
        public PartialViewResult GetDataUserSpined()
        {
            try
            {
                var week = Utilities.GetAllDayInWeek();
                var this_week = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS + " as ut")
                    .Join(ConstantsDatabase.TABLE_USERS + " as u", "ut.user_id", "u.id")
                    .Where("ut.date", ">=", week.Min(c => c))
                    .Where("ut.date", "<", week.Max(c => c).AddDays(1))
                    .Select("ut.*", "u.id_facebook", "u.full_name")
                    .Get<user_task_response>();
                foreach (var item in this_week)
                {
                    item.task = item.date.GetValueOrDefault().DayOfWeek.ToString().ToUpper(); ;
                }
                return PartialView("UserSpinedPartialView", this_week);
            }
            catch
            {
                return PartialView("UserSpinedPartialView", new List<user_task_response>());
            }
        }

        [HttpGet]
        public PartialViewResult GetDataUserNextWeek()
        {
            try
            {
                var week = Utilities.GetAllDayInWeek();
                var this_week = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS + " as ut")
                    .Join(ConstantsDatabase.TABLE_USERS + " as u", "ut.user_id", "u.id")
                    .Where("ut.date", ">=", week.Max(c => c).AddDays(1))
                    .Where("ut.date", "<", week.Max(c => c).AddDays(8))
                    .Select("ut.*", "u.id_facebook", "u.full_name")
                    .Get<user_task_response>();
                foreach (var item in this_week)
                {
                    item.task = item.date.GetValueOrDefault().DayOfWeek.ToString().ToUpper(); ;
                }
                return PartialView("NextWeekPartialView", this_week);
            }
            catch
            {
                return PartialView("NextWeekPartialView", new List<user_task_response>());
            }
        }

        [HttpGet]
        public ActionResult Spining()
        {
            try
            {
                Thread.Sleep(10000);
                var id_fb = HttpContext.Session.GetString(Constants.ID_FACEBOOK).ToString();
                var week = Utilities.GetAllDayInWeek();
                var max_date_task = DateTime.Parse(_queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS)
                    .Max<DateTime>(ConstantsDatabase.USER_TASKS_DATE).ToString("MM/dd/yyyy"));
                if(week.Max(c => c) < max_date_task)
                {
                    return Json(new { ErrorCode = 400, Message = "Bạn chỉ có thể quay trong tuần kế tiếp!", data = new List<spin_model>() });
                }    

                var check = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.USERS_ID_FACEBOOK, id_fb)
                    .FirstOrDefault<user>();
                if(check.is_spiner != 1 && check.actived != 100)
                {
                    return Json(new { ErrorCode = 400, Message = "Bạn không có quyền sử dụng!", data = new List<spin_model>() });
                }    
                var new_loop = false;
                var data = new List<spin_model>();

                var query_free_users = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.USERS_SPINED, Constants.FREE_SPIN)
                    .Where(ConstantsDatabase.USERS_STATUS, Constants.APPLIED_CHALLENGES);

                var free_users = query_free_users.Clone().Get<user>().ToList();
                if (free_users.Count() == 0)
                {
                    _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                        .Update(new
                        {
                            spined = Constants.FREE_SPIN
                        });
                    Spining();
                }
                var max_user_id = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS).Max<int>(ConstantsDatabase.COL_ID);

                for (int i = 2; i < 9; i++)
                {
                    var ran_selected = rand.Next(0, max_user_id);
                    while (free_users.FirstOrDefault(c => c.id == ran_selected) == null)
                        ran_selected = rand.Next(0, max_user_id);
                    while(i == 8 && ran_selected == check.id)
                        ran_selected = rand.Next(0, max_user_id);
                    data.Add(new spin_model()
                    {
                        day_of_week = i,
                        user_id = ran_selected
                    });
                    free_users.Remove(free_users.FirstOrDefault(c => c.id == ran_selected));
                    if (free_users.Count() == 0)
                    {
                        new_loop = true;
                        free_users = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                            .Where(ConstantsDatabase.USERS_SPINED, Constants.SPINED)
                            .Where(ConstantsDatabase.USERS_STATUS, Constants.APPLIED_CHALLENGES)
                            .Get<user>().ToList();
                        if (free_users.Count() == 0)
                        {
                            free_users = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                            .Where(ConstantsDatabase.USERS_STATUS, Constants.APPLIED_CHALLENGES)
                            .Get<user>().ToList();
                        }
                    }
                }

                if (data.Count == 7)
                {
                    var err = "";
                    var stt_update = SaveDataSpin(data, week, out err) ? 1 : 0;
                    if (stt_update > 0)
                    {
                        SaveSpin(new_loop, query_free_users);
                        return Json(new { ErrorCode = 200, Message = "success!", data = data, newLoop = new_loop });
                    }
                    return Json(new { ErrorCode = 400, Message = "error!", data = new List<spin_model>() });
                }
                return Json(new { ErrorCode = 400, Message = "error!", data = new List<spin_model>() });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorCode = 400, Message = ex.Message, data = new List<spin_model>() });
            }
        }

        private int SaveSpin(bool new_loop, SqlKata.Query query_free_users)
        {
            try
            {
                var stt_update = -1;
                var free_users_id = query_free_users.Clone().Select("id").Get<int>();
                if (new_loop)
                {
                    stt_update = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                        .WhereNotIn(ConstantsDatabase.COL_ID, free_users_id)
                        .Update(new
                        {
                            spined = Constants.FREE_SPIN
                        });
                }
                stt_update = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .WhereIn(ConstantsDatabase.COL_ID, free_users_id)
                    .Update(new
                    {
                        spined = Constants.SPINED
                    });
                var id_spin_max = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS)
                    .Max<int>(ConstantsDatabase.COL_ID);
                var user_selected = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS)
                    .Where(ConstantsDatabase.COL_ID, id_spin_max)
                    .FirstOrDefault<user_task>();
                _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Update(new
                    {
                        is_spiner = 0
                    });
                var next_spiner = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.COL_ID, user_selected.user_id)
                    .Update(new
                    {
                        is_spiner = 1
                    });
                return stt_update;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private bool SaveDataSpin(List<spin_model> data, List<DateTime> week,  out string err)
        {
            try
            {
                var status_save = -1;
                var start_date = DateTime.Parse(week.Max(c => c).AddDays(1).ToString("MM/dd/yyyy"));
                foreach (var item in data)
                {
                    status_save = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS)
                        .Where(ConstantsDatabase.USER_TASKS_DATE, ">=", start_date)
                        .Where(ConstantsDatabase.USER_TASKS_DATE, "<", start_date.AddDays(1))
                        .Delete();
                    status_save = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_TASKS)
                        .InsertGetId<int>(new
                        {
                            user_id = item.user_id,
                            date = start_date,
                            status = 0,
                            type = item.day_of_week == 8 ? 2 : 1,
                            create_date = DateTime.Now
                        });
                    start_date = start_date.AddDays(1);
                }
                err = status_save > 0 ? "success!" : "error!";
                return status_save > 0 ? true : false;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }

        [HttpGet]
        public PartialViewResult GetSpinHeader()
        {
            try
            {
                var data = new spin_role();
                var id_fb = HttpContext.Session.GetString(Constants.ID_FACEBOOK).ToString();
                var user_spiner = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.USERS_IS_SPINER, 1)
                    .FirstOrDefault<user>();
                var this_user = _queryBuilder.Query(ConstantsDatabase.TABLE_USERS)
                    .Where(ConstantsDatabase.USERS_ID_FACEBOOK, id_fb)
                    .FirstOrDefault<user>();
                if (user_spiner.id == this_user.id)
                {
                    data.is_admin = this_user.actived == 100 ? true : false;
                    data.is_spiner = true;
                    data.full_name = user_spiner.full_name;
                }    
                else if(this_user.actived == 100)
                {
                    data.is_admin = true;
                    data.is_spiner = false;
                    data.full_name = user_spiner.full_name;
                }    
                else
                {
                    data.is_admin = false;
                    data.is_spiner = false;
                    data.full_name = user_spiner.full_name;
                }
                return PartialView("RolePartialView", data);
            }
            catch (Exception ex)
            {
                return PartialView("RolePartialView", new spin_role()
                {
                    is_spiner = false,
                    full_name = "(Lấy dữ liệu lỗi rồi)"
                });
            }
        }
    }
}
