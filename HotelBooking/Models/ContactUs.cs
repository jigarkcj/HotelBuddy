using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBooking.Models
{
     public class ContactUs
    {
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string Categories { get; set; }
        public string Comment { get; set; }
        public int RequestId { get; set; }

    }
}