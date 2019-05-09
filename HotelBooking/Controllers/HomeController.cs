//This is default controller which will be called whenever application is executed
using HotelBooking.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBooking.Helpers;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;

namespace HotelBooking.Controllers
{
    public class HomeController : Controller
    {


        //These methods used to redirect to Views related to each each controllers
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult _RenderTrendingHotels()
        {

            return PartialView(GetTrendingHotelsList());
        }


        public List<HotelDetails> GetTrendingHotelsList()
        {
            // Connection string and SQL query are required to connect and get data from database
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string SQLQuery = "select * from dbo.HotelMaster order by HotelRating desc";

            //Call method from helper class
            //Return type is datatable...Pass parameters declared above
            DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

            //convert datatable to list as return type of method is in List
            List<HotelDetails> HotelDataList = new List<HotelDetails>();

            foreach (DataRow row in GetHotelData.Rows)
            {
                HotelDetails HotelObj = new HotelDetails();
                HotelObj.HotelName = row["HotelName"].ToString();
                HotelObj.HotelDescription = row["HotelDescription"].ToString();
                HotelObj.HotelFacilities = row["HotelFacilities"].ToString();
                HotelObj.HotelPrice = row["HotelPrice"].ToString();
                HotelObj.HotelRating = row["HotelRating"].ToString();
                HotelObj.HotelImage = (byte[])row["HotelImage"];
                HotelObj.HotelID = Convert.ToInt32(row["HotelID"]);
                HotelDataList.Add(HotelObj);

                Debug.WriteLine("Send to debug output." + HotelObj.HotelImage);

            }
            return HotelDataList;
        }
        //redirects to HotelBookingView
        public ActionResult HotelBookingView()
        {

            if (Session["UserID"] != null)
            {
                String[] userWishList = getUserWishlist(System.Web.HttpContext.Current.Session["UserID"].ToString());
                if (userWishList.Length > 0)
                {
                    // setting the wishlist in session
                    System.Web.HttpContext.Current.Session["UserWishList"] = userWishList;
                    ViewBag.UserWishlist = userWishList;
                }
                else
                {
                    ViewBag.UserWishlistFlag = "NULL";
                }
            }
            else
            {
                // do nothing
            }
            return View();


        }

        //redirects to RegistrationView
        public ActionResult RegistrationView()
        {
            return View();
        }

        //redirects to LoginView
        public ActionResult LoginView()
        {

            Session["PreviousURL"] = Request.UrlReferrer.AbsoluteUri;
            return View();
        }

        //redirects to ContactUsView
        public ActionResult ContactUsView()
        {

            return View();
        }

        public String[] getUserWishlist(String userID)
        {
            String[] wishListArray = { };
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            DataTable SQLDataTable = new DataTable();

            // get the wishlist hotel IDs from database for the user														
            string SQLQuery = "select WishlistHotel from dbo.UserDataTable where UserID=" + System.Web.HttpContext.Current.Session["UserID"];

            DataTable GetReviewData;
            try
            {
                GetReviewData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);
                // get the wishlist hotel IDs and store in array
                if (GetReviewData.Rows.Count != 0)
                {
                    int j = 0;
                    foreach (DataRow row in GetReviewData.Rows)
                    {
                        wishListArray = row["WishlistHotel"].ToString().Split(',');
                    }

                }
                else
                {
                    System.Diagnostics.Debug.Write("User has no wishlist yet.");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write("Exception while getting user's wishlist in Home Controller " + e.Message);

            }
            return wishListArray;
        }

        public ActionResult WishlistView()
        {
            ModelState.Clear();
            List<HotelDetails> wishListedHotels = new List<HotelDetails>();
            string[] userWishlist = getUserWishlist(Session["UserID"].ToString());
            if (userWishlist.Length > 0)
            {

                // when user has wishlisted
                foreach (var item in userWishlist)
                {
                    if (!item.Equals(""))
                    {

                        DataTable GetHotelData = Helper.getWishlistedHotelDetails(item.ToString());

                        // setting the model for the WishlistView 
                        foreach (DataRow row in GetHotelData.Rows)
                        {
                            HotelDetails hotelObj = new HotelDetails();
                            hotelObj.HotelID = Convert.ToInt32(row["HotelID"].ToString());
                            hotelObj.HotelName = row["HotelName"].ToString();
                            hotelObj.HotelDescription = row["HotelDescription"].ToString();
                            hotelObj.HotelImage = (byte[])row["HotelImage"];
                            hotelObj.HotelPrice = row["HotelPrice"].ToString();
                            hotelObj.HotelRating = row["HotelRating"].ToString();

                            wishListedHotels.Add(hotelObj);
                        }
                    }

                }

            }
            else
            {
                // when wishlist is empty
            }
            // send the updated model						 
            return View(wishListedHotels);
        }
        [HttpGet]
        public ActionResult Delete(int HotelID)
        {
            return View();
        }

        [HttpPost, ActionName("Delete")]

        public ActionResult DeleteConfirmed(int id)
        {
            // ActionResult method called to delete wishlist from User's wishlist page
            String[] userWishlist = getUserWishlist(Session["UserID"].ToString());
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            var list = new List<string>(userWishlist);
            list.Remove(id.ToString());
            userWishlist = list.ToArray();

            using (SqlConnection connection = new SqlConnection(SQLConnectionString))
            using (connection)
            {
                //connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();

                // update the user's wishlist in database
                SqlCommand command = new SqlCommand(
                    "UPDATE dbo.UserDataTable SET WishlistHotel=@HotelsToAdd WHERE UserID=@UserID", connection);
                string updatedWishlistHotelID = (string.Join(",", userWishlist.Select(x => x.ToString()).ToArray()));

                command.Connection.Open();
                // Add the parameters for the UpdateCommand.
                command.Parameters.AddWithValue("@HotelsToAdd", updatedWishlistHotelID + ",");
                command.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                command.ExecuteNonQuery();
                command.Dispose();
                adapter.InsertCommand = command;
                connection.Close();

                System.Web.HttpContext.Current.Session["UserWishList"] = userWishlist; // setting the wishlist in session
                ViewBag.UserWishlist = userWishlist;

            }
            return RedirectToAction("WishlistView", "Home"); ;
        }
    }
}