using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBooking.Models
{
    public class UserBookingDetails
    {
        public string bookingID { get; set; }
        public string paymentID { get; set; }
        public string hotelID { get; set; }
        public byte[] HotelImage { get; set; }
        public string HotelName { get; set; }
        public float HotelPrice { get; set; }
        public string hotelRating { get; set; }
        public float amount { get; set; }
        public float basePrice { get; set; }
        public string userID { get; set; }
        public string userName { get; set; }
        public string mail { get; set; }
        public string mobile { get; set; }
        public string bookingStatus { get; set; }
        public DateTime checkIn { get; set; }
        public DateTime checkOut { get; set; }
        public int rooms { get; set; }
        public int adults { get; set; }
        public int child { get; set; }
    }
}