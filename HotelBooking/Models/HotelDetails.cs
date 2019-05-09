using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBooking.Models
{
    public class HotelDetails
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }

        public string HotelCity { get; set; }

        public string HotelCountry { get; set; }

        public float Lattitude { get; set; }

        public float Longitude { get; set; }

        public string property_type { get; set; }

        public string HotelDescription { get; set; }

        public string HotelPrice { get; set; }

        public string HotelRating { get; set; }

        public byte[] HotelImage { get; set; }
        
        public string HotelFacilities { get; set; }


        
    }

    public class SearchHotelData
    {

        public string CityName { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }

        public string CallType { get; set; }

        public string RadioValue { get; set; }

        public string PropertyType { get; set; }

        public string HotelPrice { get; set; }

        public int NumberOfAdult { get; set; }

        public int NumberOfChildren { get; set; }
    }
}