using System;
using System.Data;
using HotelBooking.Helpers;
using HotelBooking.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelBooking.Controllers
{
    public class HotelDescriptionController : Controller
    {
        // GET: HotelDescription
        public ActionResult HotelDescription(string HotelID)
        {
            //Session["HotelIDVal"] = HotelID;

            ViewBag.UserIDFlag = Session["UserID"];

            if (HotelID != null)
            {
                System.Web.HttpContext.Current.Session["HotelIDVal"] = HotelID;
            }
            
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            DataTable SQLDataTable = new DataTable();

            string SQLQuery = "select * from dbo.ReviewsTable where UserID=" + System.Web.HttpContext.Current.Session["UserID"] +
            " and HotelID=" + System.Web.HttpContext.Current.Session["HotelIDVal"];

            int count = 0;
            //check if user has already submitted a review for this hotel
            DataTable GetReviewData;
            try
            {
                GetReviewData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

                foreach (var row in GetReviewData.Rows)
                {
                    count++;
                }
                // check if user has already provided reviews for the hotel
                if (count > 0)
                {
                    
                    System.Web.HttpContext.Current.Session[System.Web.HttpContext.Current.Session["HotelIDVal"].ToString()] = "Y";
                    ViewBag.HotelIDFlag = Session["HotelIDVal"];
                    ViewBag.UserReviewSubmit = Session[Session["HotelIDVal"].ToString()];
                    ViewBag.UserIDFlag = Session["UserID"];
                }
                else
                {
                    System.Web.HttpContext.Current.Session[System.Web.HttpContext.Current.Session["HotelIDVal"].ToString()] = "N";
                }

            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.Write("User has not submitted review.");
                System.Web.HttpContext.Current.Session[System.Web.HttpContext.Current.Session["HotelIDVal"].ToString()] = "N";
            }

            SQLQuery = "select Rate, ReviewMessage from dbo.ReviewsTable where HotelID=" + System.Web.HttpContext.Current.Session["HotelIDVal"] + " order by Rate desc";

            //get all the reviews from database for the hotel
            GetReviewData = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

            //store result in this list
            List<ReviewDetails> HotelReviewList = new List<ReviewDetails>();

            foreach (DataRow row in GetReviewData.Rows)
            {
                ReviewDetails HotelReviewObj = new ReviewDetails();
                HotelReviewObj.Rate = Convert.ToInt32(row["Rate"]);

                HotelReviewObj.ReviewMessage = row["ReviewMessage"].ToString();

                HotelReviewList.Add(HotelReviewObj);

            }
            if (HotelReviewList.Count < 1)
            {
                ReviewDetails HotelReviewObj = new ReviewDetails();
                HotelReviewList.Add(HotelReviewObj);
            }
            // Hotel details to be set in the model for the particular hotel ID
            string sqlQuery = "select * from dbo.HotelMaster where HotelID=" + HotelID;
            GetReviewData = Helper.GetSQLDatatable(SQLConnectionString, sqlQuery);
            foreach (DataRow row in GetReviewData.Rows)
            {

                HotelReviewList[0].HotelFacilities = row["HotelFacilities"].ToString();
                HotelReviewList[0].HotelDescription = row["HotelDescription"].ToString();
                HotelReviewList[0].HotelName = row["HotelName"].ToString();
                HotelReviewList[0].HotelImage = (byte[])row["HotelImage"];
                HotelReviewList[0].HotelPrice= float.Parse(row["HotelPrice"].ToString());
            }
            return View(HotelReviewList);

        }


        [HttpPost]
        public ActionResult Reviews(string ReviewMessage, string RatingValue)
        {
            // method to store the review details in the database
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            
            int ratingValue = Int32.Parse(RatingValue);
            ReviewDetails reviewObj = new ReviewDetails();
            reviewObj.ReviewMessage = ReviewMessage;

            using (SqlConnection connection = new SqlConnection(SQLConnectionString))
            using (connection)
            {

                //connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand command = new SqlCommand(
                    "INSERT INTO dbo.ReviewsTable(UserID, HotelID,Rate,ReviewMessage) VALUES(@UserID,@HotelID,"
                        + "@Rate,@ReviewMessage)", connection);

                command.Connection.Open();

                // Add the parameters for the InsertCommand.
                command.Parameters.AddWithValue("@UserID", Session["UserID"]);
                command.Parameters.AddWithValue("@HotelID", Session["HotelIDVal"]);
                command.Parameters.AddWithValue("@Rate", ratingValue);
                command.Parameters.AddWithValue("@ReviewMessage", ReviewMessage);

                command.ExecuteNonQuery();
                command.Dispose();

                adapter.InsertCommand = command;

                connection.Close();
            }
            return Json(new { success = true });
        }
    }
}