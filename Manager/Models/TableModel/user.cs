namespace Manager.Models.TableModel
{
    public class user
    {
        public int id { get; set; }
        public string id_facebook { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string full_name { get; set; }
        //public byte[] avatar { get; set; }
        public int? status { get; set; }
        public int? actived { get; set; }
        public int? blocked { get; set; }
        //public byte[] cover_photo { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string details { get; set; }
        public string facebook { get; set; }
        public string instagram { get; set; }
        public string twitter { get; set; }
    }
}
