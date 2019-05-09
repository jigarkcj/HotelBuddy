using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBooking.Models;
using System.Configuration;
using System.Data;
using HotelBooking.Helpers;
using System.Data.SqlClient;

namespace HotelBooking.Controllers
{
    public class SearchHotelsController : Controller
    {
        // GET: SearchHotels
        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public PartialViewResult GetHotelSearch(SearchHotelData model)
        {
            return PartialView("_RenderHotels", GetHotelSearchData(model));
        }

        private List<HotelDetails> GetHotelSearchData(SearchHotelData model)
        {
            string SQLQuery;
            Session["CheckInDate"] = model.CheckInDate;
            Session["CheckOutDate"] = model.CheckOutDate;
            Session["NumberOfAdults"] = model.NumberOfAdult;
            Session["NumberOfChildren"] = model.NumberOfChildren;
            // Connection string and SQL query are required to connect and get data from database
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string[] userWList1 = (string[])System.Web.HttpContext.Current.Session["UserWishList"];
            //Rating Filter
            if (model.RadioValue != null)
            {
                SQLQuery = "select * from dbo.HotelMaster where HotelCity='" + model.CityName + "'AND HotelRating='" + model.RadioValue + "'";

            }

            //Property Type filter
            else if (model.PropertyType != null && model.RadioValue != null)
            {
                SQLQuery = "select * from dbo.HotelMaster where HotelCity='" + model.CityName + "'AND TypeofProperty='" + model.PropertyType + "'AND HotelRating='" + model.RadioValue + "'";

            }
            else if (model.PropertyType != null)
            {
                SQLQuery = "select * from dbo.HotelMaster where HotelCity='" + model.CityName + "'AND TypeofProperty='" + model.PropertyType + "'";
            }

            //Price filter
            else if (model.HotelPrice != null)
            {
                int MinPrice = Convert.ToInt16(model.HotelPrice) - 50;
                int MaxPrice = Convert.ToInt16(model.HotelPrice);
                SQLQuery = "select * from dbo.HotelMaster where HotelCity='" + model.CityName + "' AND HotelPrice between " + MinPrice + " AND " + MaxPrice + "";
            }

            else
            {
                SQLQuery = "select * from dbo.HotelMaster where HotelCity='" + model.CityName + "'";
            }
            //Call mwthod from helper class
            //Return type is datatable...Pass parameters declared above
            DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

            //convert datatable to list as return type of method is in List
            //I have also mentioned this list type in Model
            List<HotelDetails> HotelDataList = new List<HotelDetails>();

            if (GetHotelData.Rows.Count != 0)
            {
                foreach (DataRow row in GetHotelData.Rows)
                {
                    HotelDetails HotelObj = new HotelDetails();

                    HotelObj.HotelID = (int)row["HotelID"];

                    HotelObj.HotelCity = row["HotelCity"].ToString();
                    HotelObj.HotelDescription = row["HotelDescription"].ToString();
                    HotelObj.HotelPrice = row["HotelPrice"].ToString();
                    HotelObj.HotelRating = row["HotelRating"].ToString();
                    HotelObj.HotelImage = (byte[])row["HotelImage"];
                    HotelObj.HotelName = row["HotelName"].ToString();
                    HotelDataList.Add(HotelObj);
                }
            }
            return HotelDataList;
        }

        [HttpPost]
        public ActionResult HotelSearch(SearchHotelData model)
        {

            return View(GetHotelSearchData(model));
        }

        public ActionResult WishlistHotel(string UserID, string hotelIDs)
        {
            Boolean successFlag = false;
            if (Session["UserID"] != null)
            {


                string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
                string SQLQuery = "select WishlistHotel from dbo.UserDataTable where UserID=" + Session["UserID"].ToString();
                string hotelinDB = "";
                //Call method from helper class
                //Return type is datatable...Pass parameters declared above
                DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

                //convert datatable to list as return type of method is in List
                //I have also mentioned this list type in Model
                foreach (DataRow row in GetHotelData.Rows)
                {
                    hotelinDB = row["WishlistHotel"].ToString();
                    if (hotelinDB != null || hotelinDB != "")
                    {
                        hotelinDB += hotelIDs;
                    }
                    else
                    {
                        hotelinDB = hotelIDs;
                    }
                    using (SqlConnection connection = new SqlConnection(SQLConnectionString))
                    using (connection)
                    {
                        //connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand(
                            "UPDATE dbo.UserDataTable SET WishlistHotel=@HotelsToAdd WHERE UserID=@UserID", connection);

                        command.Connection.Open();
                        // Add the parameters for the UpdateCommand.
                        command.Parameters.AddWithValue("@HotelsToAdd", hotelinDB + ",");
                        command.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                        command.ExecuteNonQuery();
                        command.Dispose();
                        adapter.InsertCommand = command;
                        connection.Close();

                        string[] userWishList = getUserWishlist(System.Web.HttpContext.Current.Session["UserID"].ToString());
                        System.Web.HttpContext.Current.Session["UserWishList"] = userWishList;
                        ViewBag.UserWishlist = userWishList;
                        successFlag = true;
                    }
                }
            }
            else
            {
             
            }

            return Json(new { success = successFlag });
        }

        public ActionResult UnWishlistHotel(string UserID, string hotelIDs)
        {
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string SQLQuery = "select WishlistHotel from dbo.UserDataTable where UserID='1'";
            string hotelinDB = "";

            //Call method from helper class
            //Return type is datatable...Pass parameters declared above
            DataTable GetHotelData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

            //convert datatable to list as return type of method is in List
            //I have also mentioned this list type in Model
            foreach (DataRow row in GetHotelData.Rows)
            {

                hotelinDB = row["WishlistHotel"].ToString();
                string[] arrayList = hotelinDB.Split(',');

                if (hotelinDB != null || hotelinDB != "")
                {
                    int indexofHotelID = Array.IndexOf(arrayList, hotelIDs);
                    arrayList = arrayList.Where(val => val != hotelIDs).ToArray();

                    if (indexofHotelID == 0)
                    {
                        arrayList = arrayList.Where(w => w != arrayList[indexofHotelID]).ToArray();    // if only one element in array
                    }
                    else if (indexofHotelID >= 1)
                    {
                        arrayList = arrayList.Where(w => w != arrayList[indexofHotelID - 1]).ToArray();
                    }
                }

                using (SqlConnection connection = new SqlConnection(SQLConnectionString))
                using (connection)
                {

                    //connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand(
                        "UPDATE dbo.UserDataTable SET WishlistHotel=@HotelsToAdd WHERE UserID=@UserID", connection);

                    command.Connection.Open();
                    string concatenated = string.Join(",", arrayList);
                    // Add the parameters for the UpdateCommand.
                    if (arrayList.Length > 0)
                    {
                        command.Parameters.AddWithValue("@HotelsToAdd", concatenated + ",");
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@HotelsToAdd", "NULL");
                    }
                    command.Parameters.AddWithValue("@UserID", 1);

                    command.ExecuteNonQuery();
                    command.Dispose();
                    adapter.InsertCommand = command;
                    connection.Close();
                }

            }
            return Json(new { success = true });
        }

        public String[] getUserWishlist(String userID)
        {
            String[] wishListArray = { };
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"]; ;
            DataTable SQLDataTable = new DataTable();
            // get the list of hotels from database
            string SQLQuery = "select WishlistHotel from dbo.UserDataTable where UserID=" + System.Web.HttpContext.Current.Session["UserID"];

            DataTable GetReviewData;
            try
            {
                GetReviewData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

                if (GetReviewData.Rows.Count != 0)
                {
                    int j = 0;
                    foreach (DataRow row in GetReviewData.Rows)
                    {
                        wishListArray = row["WishlistHotel"].ToString().Split(','); // split the Hotel IDs by comma
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
    }


}