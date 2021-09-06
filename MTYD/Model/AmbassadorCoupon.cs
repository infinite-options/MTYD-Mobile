using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class AmbassCodePost
    {
        public string code { get; set; }
        public string info { get; set; }
        public string IsGuest { get; set; }

    }

    public class AmbassadorCouponDto
    {
        public string message { get; set; }
        public string code { get; set; }
        //public double discount { get; set; }
        //public string[] uids { get; set; }
        public AmbassadorCoupon sub { get; set; }
    }


    public class AmbassadorCoupon
    {
        public string coupon_uid { get; set; }
        public string coupon_id { get; set; }
        public string valid { get; set; }
        public double threshold { get; set; }
        public double discount_percent { get; set; }
        public double discount_amount { get; set; }
        public double discount_shipping { get; set; }
        public string expire_date { get; set; }
        public int limits { get; set; }
        public string notes { get; set; }
        public int num_used { get; set; }
        public string recurring { get; set; }
        public string email_id { get; set; }
        public string cup_business_uid { get; set; }
    }

    /* 
     * for guests, pass in the address under the info section instead all things separated by commas
     * info is the email connected to the account, isGuest is checking if they are checking out as a guest
     * {"code":"pmTEST04@gmail.com","info":"bmhuss33@gmail.com","IsGuest":"False"}
     */
    public class createAmb
    {
        public string code { get; set; }
        public string info { get; set; }
        public string IsGuest { get; set; }
    }

    public class createBrandAmbClass
    {
        //isGuest is either True or False
        public void createBrandAmbassador(string code1, string info1, string isGuest1)
        {
            createAmb newAmb = new createAmb();
            newAmb.code = code1;
            newAmb.info = info1;
            newAmb.IsGuest = isGuest1;
            var createAmbSerializedObj = JsonConvert.SerializeObject(newAmb);
            var content = new StringContent(createAmbSerializedObj, Encoding.UTF8, "application/json");
            var client = new System.Net.Http.HttpClient();
            var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/brandAmbassador/create_ambassador", content);
            Console.WriteLine("RESPONSE TO CREATE_AMBASSADOR   " + response.Result);
            Console.WriteLine("CREATE JSON OBJECT BEING SENT: " + createAmbSerializedObj);
        }
    }

}
