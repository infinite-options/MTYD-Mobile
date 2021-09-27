using System;
namespace MTYD.Model
{
    public class FavsDto
    {
        public string message { get; set; }
        public string code { get; set; }
        public Favs[] result { get; set; }
    }

    public class Favs
    {
        public string favorites { get; set; }
    }

    public class UpdateFavPost
    {
        public string customer_uid { get; set; }
        public string favorite { get; set; }
    }

    public class GetFavPost
    {
        public string customer_uid { get; set; }
    }
}
