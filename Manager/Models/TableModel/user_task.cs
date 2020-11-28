using System;

namespace Manager.Models.TableModel
{
    public class user_task
    {
        public string id { get; set; }
        public int? user_id { get; set; }
        public DateTime? date { get; set; }
        public int? status { get; set; }
        public int? changed { get; set; }
        public int? type { get; set; }
        public string action_title { get; set; }
        public string action_contents { get; set; }
        public DateTime? create_date { get; set; }
    }

    public class user_task_response :user_task
    {
        public string id_facebook { get; set; }
        public string full_name { get; set; }
        public string task { get; set; }
    }
}
