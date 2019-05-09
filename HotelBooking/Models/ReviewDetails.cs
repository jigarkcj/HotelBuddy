using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBooking.Models
{
    public class ReviewDetails
    {

        public String RateID { get; set; }

        public int Rate { get; set; }

        public String UserID { get; set; }

        public String ReviewMessage { get; set; }

        public String HotelID { get; set; }

        public String averageHotelRate { get; set; }

        public String HotelDescription { get; set; }

        public String HotelFacilities { get; set; }

        public String HotelName { get; set; }

        public byte[] HotelImage { get; set; }
        public float HotelPrice { get; set; }
    }
}