using HotelBooking.Helpers;
using HotelBooking.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.Mvc;

namespace HotelBooking.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        public ActionResult HotelBooking()
        {
            //check if user is logged into website or not
            if (Session["UserName"] != null)
            {
                string hotelID = Session["HotelIDVal"].ToString();
                string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
                //getting hotel details from database
                string SQLQuery = "select * from dbo.HotelMaster where HotelID='" + hotelID + "'";
                DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);
                UserBookingDetails bookingDetails = new UserBookingDetails();
                foreach (DataRow row in GetHotelData.Rows)
                {
                    bookingDetails.hotelID = hotelID;
                    if (Session["CheckInDate"] == null)
                    {
                        bookingDetails.checkIn = DateTime.Now;
                    }else
                    {
                        bookingDetails.checkIn = DateTime.ParseExact(Session["CheckInDate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    if (Session["CheckOutDate"] == null)
                    {
                        bookingDetails.checkOut = DateTime.Now.AddDays(1);
                    }else
                    {
                        bookingDetails.checkOut = DateTime.ParseExact(Session["CheckOutDate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    Session["NumberOfAdults"] = Session["NumberOfAdults"] == null?1: Session["NumberOfAdults"];
                    Session["NumberOfChildren"] = Session["NumberOfChildren"] == null ? 0 : Session["NumberOfChildren"];
                    bookingDetails.adults = Convert.ToInt32(Session["NumberOfAdults"].ToString());
                    bookingDetails.child = Convert.ToInt32(Session["NumberOfChildren"].ToString());
                    bookingDetails.rooms = GetRooms(bookingDetails.adults, bookingDetails.child);
                    bookingDetails.HotelImage = (byte[])row["HotelImage"];
                    bookingDetails.HotelName = row["HotelName"].ToString();
                    bookingDetails.HotelPrice = float.Parse(row["HotelPrice"].ToString());
                    bookingDetails.hotelRating = row["HotelRating"].ToString();
                    bookingDetails.userName = Session["UserName"].ToString();
                    bookingDetails.mobile = Session["UserPhoneNumber"].ToString();
                    bookingDetails.mail = Session["UserEmail"].ToString();

                }
                return View(bookingDetails);
            }
            else
            {
                //redirects user to login page
                return RedirectToAction("LoginView", "Home");
            }
        }

        //calculates number of rooms required based on number of adults and child selected by the user
        public int GetRooms(int adults, int child)
        {
            int rooms = 1;
            if (adults >= child)
            {
                //only 2 adults and 2 child are allowed per room
                rooms = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(adults) / 2));
            }
            else
            {
                rooms = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(child) / 2));
            }
            return rooms;
        }

        [HttpPost]
        public ActionResult HotelPayment(string callingPage, DateTime checkInDate, DateTime checkOutDate, string rooms, string adultsHidden, string childHidden, string bookingFirstname, string bookingMobile, string bookingMail, string basePriceHidden, string totalPriceHidden, string bookingIdHidden)
        {
            UserBookingDetails userBookingDetails = new UserBookingDetails();
            userBookingDetails.checkIn = checkInDate;
            userBookingDetails.checkOut = checkOutDate;
            userBookingDetails.rooms = Convert.ToInt32(rooms);
            userBookingDetails.adults = Convert.ToInt32(adultsHidden);
            userBookingDetails.child = Convert.ToInt32(childHidden);
            userBookingDetails.userName = bookingFirstname;
            userBookingDetails.mobile = bookingMobile;
            userBookingDetails.mail = bookingMail;
            TempData["TempUserBookingDetails"] = userBookingDetails;
            TempData["BasePrice"] = float.Parse(basePriceHidden);
            TempData["PaymentAmount"] = float.Parse(totalPriceHidden);
            ViewBag.PaymentAmountToPay = float.Parse(totalPriceHidden);
            if (callingPage == "ModifyBooking")
            {
                //this falg is used to differentiate between modify and new booking
                TempData["UpdateUserPaymentDetails"] = "True";
            }
            else
            {
                TempData["UpdateUserPaymentDetails"] = "False";
            }
            return View();
        }

        // POST: HotelPayment
        [HttpPost]
        public ActionResult Payment(string stripeToken, string cardType)
        {
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            PaymentChargeDetails chargeModel = new PaymentChargeDetails();
            //deducts the money from the card using stripe API
            chargeModel = CreateCharge(stripeToken, cardType, TempData["PaymentAmount"].ToString());
            if (chargeModel.status == "succeeded")
            {
                string updateUserPaymentDetails = TempData["UpdateUserPaymentDetails"].ToString();
                UserBookingDetails TempUserBookingDetails = TempData["TempUserBookingDetails"] as UserBookingDetails;
                UserBookingDetails userBookingDetails = new UserBookingDetails();
                userBookingDetails.paymentID = chargeModel.chargeId;
                userBookingDetails.hotelID = Session["HotelIDVal"].ToString();
                userBookingDetails.userID = Session["UserID"].ToString();
                userBookingDetails.userName = TempUserBookingDetails.userName;
                userBookingDetails.mail = TempUserBookingDetails.mail;
                userBookingDetails.mobile = TempUserBookingDetails.mobile;
                userBookingDetails.bookingStatus = "Confirmed";
                userBookingDetails.checkIn = TempUserBookingDetails.checkIn;
                userBookingDetails.checkOut = TempUserBookingDetails.checkOut;
                userBookingDetails.rooms = TempUserBookingDetails.rooms;
                userBookingDetails.adults = TempUserBookingDetails.adults;
                userBookingDetails.child = TempUserBookingDetails.child;
                userBookingDetails.basePrice = float.Parse(TempData["BasePrice"].ToString());
                if (updateUserPaymentDetails == "True")
                {
                    // updating payment details to database
                    userBookingDetails.bookingID = Session["bookingID"].ToString();
                    Session["bookingID"] = userBookingDetails.bookingID;
                    Helper.UpdateUserBookingDetails(SQLConnectionString, userBookingDetails);
                }
                else
                {
                    // inserting payment details to database
                    userBookingDetails.bookingID = Guid.NewGuid().ToString("N");
                    Session["bookingID"] = userBookingDetails.bookingID;
                    Helper.InsertIntoUserBookingDetails(SQLConnectionString, userBookingDetails);
                }

                return View("PaymentSuccess", chargeModel);
            }
            else
            {
                return View("PaymentFailure", chargeModel);
            }

        }
        public PaymentChargeDetails CreateCharge(string stripeToken, string cardType, string paymentAmount)
        {
            string userID = Session["UserID"].ToString();
            string hotelID = Session["HotelIDVal"].ToString();
            float amount = float.Parse(paymentAmount) * 100;
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeSecretKey"]);

            // Token is created using Checkout or Elements!
            // Get the payment token submitted by the form:Using ASP.NET MVC
            var token = stripeToken;
            var options = new ChargeCreateOptions
            {
                Amount = (long)amount,
                Currency = "cad",
                Description = "charge from HotelBuddy for user " + userID,
                SourceId = token,
            };
            var service = new ChargeService();
            Charge charge;
            PaymentChargeDetails chargeModel = new PaymentChargeDetails();
            try
            {
                // stripe paymentAPI checks card deatisl and creates charge if payment is successful
                charge = service.Create(options);
                //chargeID is unique
                chargeModel.chargeId = charge.Id;
                chargeModel.status = charge.Status;
            }
            catch (StripeException e)
            {
                switch (e.StripeError.ErrorType)
                {
                    case "card_error":
                        Console.WriteLine("Code: " + e.StripeError.Code);
                        Console.WriteLine("Message: " + e.StripeError.Message);
                        chargeModel.message = e.StripeError.Message;
                        break;
                    case "api_connection_error":
                        break;
                    case "api_error":
                        break;
                    case "authentication_error":
                        chargeModel.message = e.StripeError.Message;
                        break;
                    case "invalid_request_error":
                        break;
                    case "rate_limit_error":
                        chargeModel.message = e.StripeError.Message;
                        break;
                    case "validation_error":
                        chargeModel.message = e.StripeError.Message;
                        break;
                    default:
                        // Unknown Error Type
                        break;
                }
            }
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            chargeModel.hotelID = hotelID;
            chargeModel.userID = userID;
            chargeModel.amount = float.Parse(TempData["PaymentAmount"].ToString());
            chargeModel.cardType = cardType;
            chargeModel.paymentDate = DateTime.Now;
            if (chargeModel.status == "succeeded")
            {
                chargeModel.message = "N/A";
            }
            else
            {
                chargeModel.chargeId = "PF_" + Guid.NewGuid().ToString("N");
                chargeModel.status = "failed";
            }
            // insert payment details with payment status and reason into database
            Helper.InsertIntoPaymentDataBase(SQLConnectionString, chargeModel);
            return chargeModel;
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }
        public ActionResult PaymentFailure()
        {
            return View();
        }
        public ActionResult BookedDetails()
        {
            string bookingID = Session["bookingID"].ToString();
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            //getting the booked details from database
            string SQLQuery = "select booking.BookingID,hotel.HotelName,booking.CheckIn,booking.CheckOut,booking.Rooms," +
                "booking.Adults,booking.Child,booking.NameOnCard,booking.Mail,booking.mobile,booking.BasePrice,hotel.HotelImage,payment.Amount,hotel.HotelRating " +
                "from dbo.UserBookingDetails booking Inner Join dbo.PaymentInfo payment on booking.PaymentID = payment.PaymentID " +
                "Inner Join dbo.HotelMaster hotel on booking.HotelID = hotel.HotelID" +
                " where booking.BookingID='" + bookingID + "'";
            DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);
            UserBookingDetails bookedDetails = new UserBookingDetails();
            foreach (DataRow row in GetHotelData.Rows)
            {
                bookedDetails.bookingID = bookingID;
                bookedDetails.HotelName = row["HotelName"].ToString();
                bookedDetails.checkIn = DateTime.Parse(row["CheckIn"].ToString());
                bookedDetails.checkOut = DateTime.Parse(row["CheckOut"].ToString());
                bookedDetails.rooms = int.Parse(row["Rooms"].ToString());
                bookedDetails.adults = int.Parse(row["Adults"].ToString());
                bookedDetails.child = int.Parse(row["Child"].ToString());
                bookedDetails.userName = row["NameOnCard"].ToString();
                bookedDetails.mail = row["Mail"].ToString();
                bookedDetails.mobile = row["mobile"].ToString();
                bookedDetails.HotelImage = (byte[])row["HotelImage"];
                bookedDetails.basePrice = float.Parse(row["BasePrice"].ToString());
                bookedDetails.hotelRating = row["HotelRating"].ToString();
            }
            return View(bookedDetails);
        }
        public ActionResult BookingDetailsList()
        {
            string userID = Session["UserID"].ToString();
            List<UserBookingDetails> bookedDetailsList = new List<UserBookingDetails>();
            //AllBookingDetails method returns a list containing all the bookings of the user
            bookedDetailsList = AllBookingDetails(userID);
            return View(bookedDetailsList);
        }
        public ActionResult ModifyBooking(string bookingID)
        {
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            Session["bookingID"] = bookingID;
            //to differentiate modify from new booking
            TempData["UpdateUserPaymentDetails"] = "True";
            //checking booking status and checkin again in the query to make sure even if user adds the modify button in the html 
            // using inspect element the website wont allow for the modification
            string SQLQuery = "select booking.BookingID,booking.HotelID,hotel.HotelName,booking.CheckIn,booking.CheckOut,booking.Rooms," +
                "booking.Adults,booking.Child,booking.NameOnCard,booking.Mail,booking.mobile,booking.BasePrice,hotel.HotelImage,payment.Amount,hotel.HotelPrice,hotel.HotelRating " +
                "from dbo.UserBookingDetails booking Inner Join dbo.PaymentInfo payment on booking.PaymentID = payment.PaymentID " +
                "Inner Join dbo.HotelMaster hotel on booking.HotelID = hotel.HotelID" +
                " where booking.BookingID='" + bookingID + "' and booking.BookingStatus='Confirmed' and booking.CheckIn > DATEADD(day,1,GETDATE())";
            DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);
            UserBookingDetails bookedDetails = new UserBookingDetails();
            if (GetHotelData.Rows.Count > 0)
            {
                foreach (DataRow row in GetHotelData.Rows)
                {
                    bookedDetails.bookingID = bookingID;
                    bookedDetails.HotelName = row["HotelName"].ToString();
                    bookedDetails.checkIn = DateTime.Parse(row["CheckIn"].ToString());
                    bookedDetails.checkOut = DateTime.Parse(row["CheckOut"].ToString());
                    bookedDetails.rooms = int.Parse(row["Rooms"].ToString());
                    bookedDetails.adults = int.Parse(row["Adults"].ToString());
                    bookedDetails.child = int.Parse(row["Child"].ToString());
                    bookedDetails.userName = row["NameOnCard"].ToString();
                    bookedDetails.mail = row["Mail"].ToString();
                    bookedDetails.mobile = row["mobile"].ToString();
                    bookedDetails.HotelImage = (byte[])row["HotelImage"];
                    bookedDetails.basePrice = float.Parse(row["BasePrice"].ToString());
                    bookedDetails.HotelPrice = float.Parse(row["HotelPrice"].ToString());
                    bookedDetails.hotelRating = row["HotelRating"].ToString();
                    Session["HotelIDVal"] = row["HotelID"].ToString();
                }
                return View(bookedDetails);
            }
            else
            {
                //returns to booking history if there is no data or modification is not possible for the data
                List<UserBookingDetails> bookedDetailsList = new List<UserBookingDetails>();
                bookedDetailsList = AllBookingDetails(Session["UserID"].ToString());
                return View("BookingDetailsList", bookedDetailsList);
            }
        }
        [HttpPost]
        public ActionResult ModifyUserDetails(DateTime checkInDate, DateTime checkOutDate, string rooms, string adultsHidden, string childHidden, string bookingFirstname, string bookingMobile, string bookingMail, string bookingIdHidden)
        {
            string bookingID = bookingIdHidden;
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            UserBookingDetails userBookingDetails = new UserBookingDetails();
            userBookingDetails.userName = bookingFirstname;
            userBookingDetails.bookingID = bookingID;
            userBookingDetails.mobile = bookingMobile;
            userBookingDetails.mail = bookingMail;
            userBookingDetails.userID = Session["UserID"].ToString();
            userBookingDetails.checkIn = checkInDate;
            userBookingDetails.checkOut = checkOutDate;
            userBookingDetails.rooms = int.Parse(rooms);
            userBookingDetails.adults = int.Parse(adultsHidden);
            userBookingDetails.child = int.Parse(childHidden);
            //updating the values in database
            Helper.UpdateOnlyUserBookingDetails(SQLConnectionString, userBookingDetails);
            List<UserBookingDetails> bookedDetailsList = new List<UserBookingDetails>();
            bookedDetailsList = AllBookingDetails(userBookingDetails.userID);
            return View("BookingDetailsList", bookedDetailsList);

        }
        public ActionResult CancelBooking(string bookingID)
        {
            UserBookingDetails cancelBookingDetails = new UserBookingDetails();
            cancelBookingDetails.userID = Session["UserID"].ToString();
            cancelBookingDetails.bookingID = bookingID;
            cancelBookingDetails.bookingStatus = "Cancelled";
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            //updating cancel status in the database
            Helper.CancelBooking(SQLConnectionString, cancelBookingDetails);
            string SQLQuery = "select booking.BookingID,hotel.HotelName,booking.CheckIn,booking.CheckOut,booking.Rooms," +
                "booking.Adults,booking.Child,booking.NameOnCard,booking.Mail,booking.mobile,booking.BasePrice,hotel.HotelImage,payment.Amount,hotel.HotelRating " +
                "from dbo.UserBookingDetails booking Inner Join dbo.PaymentInfo payment on booking.PaymentID = payment.PaymentID " +
                "Inner Join dbo.HotelMaster hotel on booking.HotelID = hotel.HotelID" +
                " where booking.BookingID='" + bookingID + "'";
            DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);
            UserBookingDetails bookedDetails = new UserBookingDetails();
            foreach (DataRow row in GetHotelData.Rows)
            {
                bookedDetails.bookingID = bookingID;
                bookedDetails.HotelName = row["HotelName"].ToString();
                bookedDetails.checkIn = DateTime.Parse(row["CheckIn"].ToString());
                bookedDetails.checkOut = DateTime.Parse(row["CheckOut"].ToString());
                bookedDetails.rooms = int.Parse(row["Rooms"].ToString());
                bookedDetails.adults = int.Parse(row["Adults"].ToString());
                bookedDetails.child = int.Parse(row["Child"].ToString());
                bookedDetails.userName = row["NameOnCard"].ToString();
                bookedDetails.mail = row["Mail"].ToString();
                bookedDetails.mobile = row["mobile"].ToString();
                bookedDetails.HotelImage = (byte[])row["HotelImage"];
                bookedDetails.basePrice = float.Parse(row["BasePrice"].ToString());
                bookedDetails.amount = bookedDetails.basePrice + (float)(bookedDetails.basePrice * 0.15);
                bookedDetails.hotelRating = row["HotelRating"].ToString();
            }
            return View(bookedDetails);
        }
        public List<UserBookingDetails> AllBookingDetails(string userID)
        {
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            //get all the booking details list for the user
            string SQLQuery = "select booking.BookingID,hotel.HotelName,booking.CheckIn,booking.CheckOut,booking.Rooms," +
                "booking.Adults,booking.Child,booking.NameOnCard,booking.Mail,booking.mobile,booking.BasePrice,hotel.HotelImage,payment.Amount,booking.BookingStatus,hotel.HotelRating " +
                "from dbo.UserBookingDetails booking Inner Join dbo.PaymentInfo payment on booking.PaymentID = payment.PaymentID " +
                "Inner Join dbo.HotelMaster hotel on booking.HotelID = hotel.HotelID " +
                "where booking.UserID='" + userID + "' order by CheckIn DESC";
            DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);
            List<UserBookingDetails> bookedDetailsList = new List<UserBookingDetails>();
            foreach (DataRow row in GetHotelData.Rows)
            {
                UserBookingDetails bookedDetails = new UserBookingDetails();
                bookedDetails.bookingID = row["BookingID"].ToString();
                bookedDetails.HotelName = row["HotelName"].ToString();
                bookedDetails.checkIn = DateTime.Parse(row["CheckIn"].ToString());
                bookedDetails.checkOut = DateTime.Parse(row["CheckOut"].ToString());
                bookedDetails.bookingStatus = row["BookingStatus"].ToString();
                bookedDetails.rooms = int.Parse(row["Rooms"].ToString());
                bookedDetails.adults = int.Parse(row["Adults"].ToString());
                bookedDetails.child = int.Parse(row["Child"].ToString());
                bookedDetails.userName = row["NameOnCard"].ToString();
                bookedDetails.mail = row["Mail"].ToString();
                bookedDetails.mobile = row["mobile"].ToString();
                bookedDetails.HotelImage = (byte[])row["HotelImage"];
                bookedDetails.hotelRating = row["HotelRating"].ToString();
                bookedDetails.basePrice = float.Parse(row["BasePrice"].ToString());
                bookedDetails.amount = bookedDetails.basePrice + (bookedDetails.basePrice * 15 / 100);
                bookedDetailsList.Add(bookedDetails);
            }
            return bookedDetailsList;
        }
    }
}