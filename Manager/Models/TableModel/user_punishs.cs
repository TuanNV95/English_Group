using System;

namespace Manager.Models.TableModel
{
    public class user_punishs
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public string punish_contents { get; set; }
        public DateTime? date_created { get; set; }
        public int? user_created { get; set; }
        public int? status { get; set; }
    }
    public class user_punishs_response : user_punishs
    {
        public string id_facebook { get; set; }
        public string full_name { get; set; }
        public int sum { get; set; }
    }

    public class top_punish
    {
        public int? user_id { get; set; }
        public int? punish_sum { get; set; }
        public string full_name { get; set; }
    }
}
