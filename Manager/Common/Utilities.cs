using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Common
{
    public static  class Utilities
    {
        public static List<DateTime> GetAllDayInWeek()
        {
            try
            {
                DateTime today = DateTime.Today;
                int currentDayOfWeek = (int)today.DayOfWeek;
                DateTime sunday = today.AddDays(-currentDayOfWeek);
                DateTime monday = sunday.AddDays(1);
                // If we started on Sunday, we should actually have gone *back*
                // 6 days instead of forward 1...
                if (currentDayOfWeek == 0)
                {
                    monday = monday.AddDays(-7);
                }
                return Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
            }
            catch
            {
                return new List<DateTime>();
            }
        }
    }
}
