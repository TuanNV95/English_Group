namespace Manager.Connection
{
    public class ConstantsDatabase
    {
        public const string TABLE_USERS = "users";
        public const string TABLE_USER_TASKS = "user_tasks";
        public const string TABLE_USER_PUNISHS = "user_punishs";
        public const string TABLE_USER_NOTIFICATIONS = "user_notifications";


        public const string V_TOP_PUNISH = "v_top_punish";


        public const string COL_ID = "id";

        // Columns in USERS table
        public const string USERS_ID_FACEBOOK = "id_facebook";
        public const string USERS_USER_NAME = "user_name";
        public const string USERS_PASSWORD = "password";
        public const string USERS_FULL_NAME = "full_name";
        public const string USERS_AVATAR = "avatar";
        public const string USERS_STATUS = "status";
        public const string USERS_ACTIVED = "actived";
        public const string USERS_BLOCKED = "blocked";
        public const string USERS_COVER_PHOTO = "cover_photo";
        public const string USERS_EMAIL = "email";
        public const string USERS_PHONE = "phone";
        public const string USERS_details = "details";
        public const string USERS_FACEBOOK = "facebook";
        public const string USERS_INSTAGRAM = "instagram";
        public const string USERS_TWITTER = "twitter";
        public const string USERS_SPINED = "spined";
        public const string USERS_IS_SPINER = "is_spiner";

        // Columns in USER_TASKS tables
        public const string USER_TASKS_USER_ID = "user_id";
        public const string USER_TASKS_DATE = "date";
        public const string USER_TASKS_STATUS = "status";
        public const string USER_TASKS_CHANGED = "changed";
        public const string USER_TASKS_TYPE = "type";
        public const string USER_TASKS_ACTION_TITLE = "action_title";
        public const string USER_TASKS_ACTION_CONTENTS = "action_contents";
        public const string USER_TASKS_CREATE_DATE = "create_date";

        // Columns in USER_PUNISHS table
        public const string USER_PUNISHS_USER_ID = "user_id";
        public const string USER_PUNISHS_PUNISH_CONTENTS = "punish_contents";
        public const string USER_PUNISHS_DATE_CREATED = "date_created";
        public const string USER_PUNISHS_USER_CREATED = "user_created";

        // Column in USER_NOTIFICATIONS table
        public const string USER_NOTIFICATIONS_USER_ID = "user_id";
        public const string USER_NOTIFICATIONS_NOTIFICATION = "notification";
        public const string USER_NOTIFICATIONS_DATE_CREATED = "date_created";
        public const string USER_NOTIFICATIONS_STATUS = "status";
    }
}
