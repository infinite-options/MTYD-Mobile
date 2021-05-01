using System;
namespace MTYD.Model.Login.LoginClasses
{
    public class RDSLogInMessage
    {
        public string message { get; set; }
        public int code { get; set; }
        public string result { get; set; }
        public string sql { get; set; }
    }
}
