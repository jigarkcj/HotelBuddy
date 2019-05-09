using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using HotelBooking.Models;
using System.Configuration;
using System.Web.Security;

namespace HotelBooking.Helpers
{
    public class Helper
    {
        public static DataTable GetSQLDatatable(string ConnectionString, string SQLQuery)
        {
            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {

                connection.Open();
                SqlCommand cmd = new SqlCommand(SQLQuery, connection);
                // check connection
                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(SQLDataTable);
                connection.Close();
                da.Dispose();
            }
            return SQLDataTable;
        }


        /**Method created to insert data taken from controller into database**/
        public static Boolean insertContactInfo(string ConnectionString, string SQLQuery)
        {
            try
            {
                //Boolean inserted=false;
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                using (connection)
                {


                    SqlDataAdapter da = new SqlDataAdapter();
                    da.InsertCommand = new SqlCommand(SQLQuery, connection);
                    // check connection
                    // create data adapter

                    connection.Open();
                    da.InsertCommand.ExecuteNonQuery();
                    connection.Close();

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }


        }
        public static void InsertIntoPaymentDataBase(string ConnectionString, PaymentChargeDetails paymentChargeDetails)
        {
            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {

                connection.Open();
                SqlCommand command;
                string SQLQuery = "insert into dbo.PaymentInfo values (@PaymentID,@HotelID,@UserID,@PaymentStatus,@Amount,"
                                 + " @CardType,@PaymentDate,@Reason)";
                command = new SqlCommand(SQLQuery, connection);
                command.Parameters.AddWithValue("@PaymentID", paymentChargeDetails.chargeId);
                command.Parameters.AddWithValue("@HotelID", paymentChargeDetails.hotelID);
                command.Parameters.AddWithValue("@UserID", paymentChargeDetails.userID);
                command.Parameters.AddWithValue("@PaymentStatus", paymentChargeDetails.status);
                command.Parameters.AddWithValue("@Amount", paymentChargeDetails.amount);
                command.Parameters.AddWithValue("@CardType", paymentChargeDetails.cardType);
                command.Parameters.AddWithValue("@PaymentDate", paymentChargeDetails.paymentDate);
                command.Parameters.AddWithValue("@Reason", paymentChargeDetails.message);
                // this will insert data into database based on SQLQuery value
                command.ExecuteNonQuery();
                //release resources
                command.Dispose();
                //close connection
                connection.Close();
            }
        }
        public static void InsertIntoUserBookingDetails(string ConnectionString, UserBookingDetails userBookingDetails)
        {
            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {

                connection.Open();
                SqlCommand command;
                string SQLQuery = "insert into dbo.UserBookingDetails values (@BookingID,@PaymentID,@HotelID,@UserID,@NameOnCard,@Mail,"
                                 + " @Mobile,@BookingStatus,@CheckIn,@CheckOut,@Rooms,@Adults,@Child,@BasePrice)";
                command = new SqlCommand(SQLQuery, connection);
                command.Parameters.AddWithValue("@BookingID", userBookingDetails.bookingID);
                command.Parameters.AddWithValue("@PaymentID", userBookingDetails.paymentID);
                command.Parameters.AddWithValue("@HotelID", userBookingDetails.hotelID);
                command.Parameters.AddWithValue("@UserID", userBookingDetails.userID);
                command.Parameters.AddWithValue("@NameOnCard", userBookingDetails.userName);
                command.Parameters.AddWithValue("@Mail", userBookingDetails.mail);
                command.Parameters.AddWithValue("@Mobile", userBookingDetails.mobile);
                command.Parameters.AddWithValue("@BookingStatus", userBookingDetails.bookingStatus);
                command.Parameters.AddWithValue("@CheckIn", userBookingDetails.checkIn);
                command.Parameters.AddWithValue("@CheckOut", userBookingDetails.checkOut);
                command.Parameters.AddWithValue("@Rooms", userBookingDetails.rooms);
                command.Parameters.AddWithValue("@Adults", userBookingDetails.adults);
                command.Parameters.AddWithValue("@Child", userBookingDetails.child);
                command.Parameters.AddWithValue("@BasePrice", userBookingDetails.basePrice);
                command.ExecuteNonQuery();
                //release resources
                command.Dispose();
                //close connection
                connection.Close();
            }
        }
        public static void UpdateUserBookingDetails(string ConnectionString, UserBookingDetails userBookingDetails)
        {
            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {

                connection.Open();
                SqlCommand command;
                string SQLQuery = "update dbo.UserBookingDetails set PaymentID=@PaymentID,NameOnCard=@NameOnCard,Mail=@Mail,"
                                 + " mobile=@Mobile,CheckIn=@CheckIn,CheckOut=@CheckOut,Rooms=@Rooms,Adults=@Adults,Child=@Child,BasePrice=@BasePrice where BookingID='" + userBookingDetails.bookingID + "'";
                command = new SqlCommand(SQLQuery, connection);
                command.Parameters.AddWithValue("@PaymentID", userBookingDetails.paymentID);
                command.Parameters.AddWithValue("@NameOnCard", userBookingDetails.userName);
                command.Parameters.AddWithValue("@Mail", userBookingDetails.mail);
                command.Parameters.AddWithValue("@Mobile", userBookingDetails.mobile);
                command.Parameters.AddWithValue("@CheckIn", userBookingDetails.checkIn);
                command.Parameters.AddWithValue("@CheckOut", userBookingDetails.checkOut);
                command.Parameters.AddWithValue("@Rooms", userBookingDetails.rooms);
                command.Parameters.AddWithValue("@Adults", userBookingDetails.adults);
                command.Parameters.AddWithValue("@Child", userBookingDetails.child);
                command.Parameters.AddWithValue("@BasePrice", userBookingDetails.basePrice);
                command.ExecuteNonQuery();
                //release resources
                command.Dispose();
                //close connection
                connection.Close();
            }
        }
        public static void UpdateOnlyUserBookingDetails(string ConnectionString, UserBookingDetails userBookingDetails)
        {
            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {

                connection.Open();
                SqlCommand command;
                string SQLQuery = "update dbo.UserBookingDetails set NameOnCard=@NameOnCard,Mail=@Mail,"
                                 + " mobile=@Mobile where BookingID='" + userBookingDetails.bookingID + "'";
                command = new SqlCommand(SQLQuery, connection);
                command.Parameters.AddWithValue("@NameOnCard", userBookingDetails.userName);
                command.Parameters.AddWithValue("@Mail", userBookingDetails.mail);
                command.Parameters.AddWithValue("@Mobile", userBookingDetails.mobile);
                command.ExecuteNonQuery();
                //release resources
                command.Dispose();
                //close connection
                connection.Close();
            }
        }
        public static void CancelBooking(string ConnectionString, UserBookingDetails userBookingDetails)
        {
            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {

                connection.Open();
                SqlCommand command;
                string SQLQuery = "update dbo.UserBookingDetails set BookingStatus=@BookingStatus where BookingID=@BookingID and UserID=@UserID";
                command = new SqlCommand(SQLQuery, connection);
                command.Parameters.AddWithValue("@BookingStatus", userBookingDetails.bookingStatus);
                command.Parameters.AddWithValue("@BookingID", userBookingDetails.bookingID);
                command.Parameters.AddWithValue("@UserID", userBookingDetails.userID);
                command.ExecuteNonQuery();
                //release resources
                command.Dispose();
                //close connection
                connection.Close();
            }
        }

        public static DataTable CheckValidUser(string UserName, String Password)
        {

            //HashPasswordForStoringInConfigFile() method produces hash for the imput password passed, To hash the password Message Digest 5 (MD5) algorithm was used. 
            string hashed_password = FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5");

            string SQLQuery = "select * from dbo.UserDataTable where UserEmail = '" + UserName + "' AND UserPassword = '" + hashed_password + "';";
            string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {

                connection.Open();
                SqlCommand cmd = new SqlCommand(SQLQuery, connection);
                // check connection
                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(SQLDataTable);
                connection.Close();
                da.Dispose();
            }

            return SQLDataTable;

        }

        public static DataTable getWishlistedHotelDetails(string hotelID)
        {
            string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            DataTable SQLDataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (connection)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select HotelID, HotelName, HotelDescription, HotelPrice, HotelRating, HotelImage from dbo.HotelMaster where HotelID=@HotelId", connection);
                cmd.Parameters.AddWithValue("@HotelId", hotelID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(SQLDataTable);
                connection.Close();
                da.Dispose();
            }
            return SQLDataTable;
        }
    }

}