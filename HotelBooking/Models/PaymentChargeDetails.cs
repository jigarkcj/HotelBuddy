using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBooking.Models
{
    public class PaymentChargeDetails
    {
        public string chargeId { get; set; }
        public string hotelID { get; set; }
        public string userID { get; set; }
        public string status { get; set; }
        public float amount { get; set; }
        public string cardType { get; set; }
        public DateTime paymentDate { get; set; }
        public string message { get; set; }

    }
}