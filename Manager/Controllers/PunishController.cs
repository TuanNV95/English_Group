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
    public class PunishController : _DbExcute
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
        public PartialViewResult GetDataTopPunish()
        {
            try
            {
                var data = _queryBuilder.Query(ConstantsDatabase.V_TOP_PUNISH)
                    .Get<top_punish>().ToList();
                var lst_user_id = data.Select(c => c.user_id);
                return PartialView("TopPartialView", data);
            }
            catch
            {
                return PartialView("TopPartialView", new List<top_punish>());
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
                    .Where("status", 0)
                    .WhereIn("user_id", lst_user_id).Get<user_punishs>().ToList();
                foreach (var item in data)
                {
                    item.sum = lst_sum.Where(c => c.user_id == item.user_id).Count();
                }
                return PartialView("PunishNowPartialView", data.GroupBy(c => c.user_id).Select(c =>c.FirstOrDefault()).OrderByDescending(c =>c.sum).ToList());
            }
            catch
            {
                return PartialView("PunishNowPartialView", new List<user_punishs_response>());
            }
        }

        [HttpGet]
        public PartialViewResult GetDataPunishHistory()
        {
            try
            {
                var start_month = DateTime.Parse(DateTime.Now.Month + "/1/" + DateTime.Now.Year);
                var start_next_month = DateTime.Parse(DateTime.Now.AddMonths(1).Month + "/1/" + (DateTime.Now.Month == 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year));
                var data = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS + " as up")
                    .Join(ConstantsDatabase.TABLE_USERS + " as u", "up.user_id", "u.id")
                    .Where("up.date_created", ">=", start_month)
                    .Where("up.date_created", "<", start_next_month)
                    .Select("up.*", "u.id_facebook", "u.full_name")
                    .OrderByDesc("up.date_created")
                    .Get<user_punishs_response>().ToList();
                var lst_user_id = data.Select(c => c.user_id);
                var lst_sum = _queryBuilder.Query(ConstantsDatabase.TABLE_USER_PUNISHS)
                    .Where("date_created", ">=", start_month)
                    .Where("date_created", "<", start_next_month)
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
    }
}
