using System;
namespace MTYD.Model
{
    public class AlertsObj
    {
        public string message { get; set; }
        public string code { get; set; }
        public Alerts[] result { get; set; }
    }

    public class Alerts
    {
        public string alert_uid { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public string responses { get; set; }
    }
}
