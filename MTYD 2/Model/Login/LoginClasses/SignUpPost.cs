using System;
namespace MTYD.Model.Login.LoginClasses
{
    public class SignUpPost
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string address { get; set; }
        public string unit { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string referral_source { get; set; }
        public string role { get; set; }
        public string social { get; set; }
        public string password { get; set; }
        public string mobile_access_token { get; set; }
        public string mobile_refresh_token { get; set; }
        public string user_access_token { get; set; }
        public string user_refresh_token { get; set; }
        public string social_id { get; set; }
        public string cust_id { get; set; }
    }

    public class changePasswordPost
    {
        public string customer_uid { get; set; }
        public string old_password { get; set; }
        public string new_password { get; set; }
    }

    public class emailVerifyPost
    {
        public string email { get; set; }
    }
}
