using HotelBooking.Helpers;
using HotelBooking.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HotelBooking.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetRegistrationDetails(string email_id, string password)
        {
            //Debug.WriteLine("full name in get");

            return View();
        }
        [HttpPost]
        public ActionResult GetRegistrationDetails(string full_name, string email_id, string phone_number, string password, string confirmPass)
        {
            Registration user_register = new Registration();
            user_register.UserName = full_name;
            user_register.UserEmail = email_id;
            user_register.UserPhoneNumber = phone_number;
            user_register.UserPassword = password;
            user_register.UserConfirmPassword = confirmPass;

            //HashPasswordForStoringInConfigFile() method produces hash for the imput password passed, To hash the password Message Digest 5 (MD5) algorithm was used. 
            string hashed_password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
            
            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string SQLQuery = "SELECT * FROM dbo.UserDataTable where UserEmail ='" + email_id + "' ;";
            DataTable UserExist = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);
            Registration LoginObj = new Registration();
            foreach (DataRow row in UserExist.Rows)
            {
                LoginObj.UserEmail = row["UserEmail"].ToString();
            }

            if (email_id.Equals(LoginObj.UserEmail))
            {
                ViewBag.UserExist = "Email ID Already Exists!! Please use a different one to create and account.";
                return Redirect("../Registration/GetRegistrationDetails");
            }
            else
            {
                string SQLConnectionString1 = ConfigurationManager.AppSettings["ConnectionString"];
                string SQLQuery1 = "Insert into dbo.UserDataTable (UserName,UserEmail,UserPhoneNumber,UserPassword) values ('" + user_register.UserName + "' ,'" + user_register.UserEmail + "','" + user_register.UserPhoneNumber + "','" + hashed_password + "')";

                DataTable SetRegistrationValues = Helper.GetSQLDatatable(SQLConnectionString1, SQLQuery1);
            }
            return View("../Home/LoginView");
        }
        
        public ActionResult LoginRedirect()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginRedirect(string email_id, string password)
        {

            Registration login_register = new Registration();

            login_register.UserEmail = email_id;
            login_register.UserPassword = password;

            //HashPasswordForStoringInConfigFile() method produces hash for the imput password passed, To hash the password Message Digest 5 (MD5) algorithm was used. 
            string hashedpass = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");

            string SQLConnectionString = "Data Source=localhost;Initial Catalog=HotelBuddy;Integrated Security=True";
            string SQLQuery = "SELECT * FROM dbo.UserDataTable where UserEmail ='" + email_id + "' ;";
            DataTable GetLoginDetails = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

            //convert datatable to list as return type of method is in List
            List<Registration> LoginDataList = new List<Registration>();

            foreach (DataRow row in GetLoginDetails.Rows)
            {
                Registration LoginObj = new Registration();
                LoginObj.UserEmail = row["UserEmail"].ToString();
                LoginObj.UserName = row["UserName"].ToString();
                LoginObj.UserPassword = row["UserPassword"].ToString();
                LoginObj.UserPhoneNumber = row["UserPhoneNumber"].ToString();
                LoginDataList.Add(LoginObj);
                Session["username"] = LoginObj.UserName;
                Session["useremail"] = LoginObj.UserEmail;
                Session["userpassword"] = password;
                Session["userphonenumber"] = LoginObj.UserPhoneNumber;
                //Debug.WriteLine("Send to debug output." + LoginDataList);

                if ((login_register.UserEmail.Equals(LoginObj.UserEmail)) && (hashedpass.Equals(LoginObj.UserPassword)))
                {
                    System.Web.HttpContext.Current.Session["UserName"] = LoginObj.UserName;
                    return Redirect("../Home/HotelBookingView");
                }
                else
                {
                    //ViewBag.CredentialsInvalid = "Invalid User Credentials";
                    ViewData["CredentialsInvalid"] = "Invalid User Credentials";
                    return Redirect("../Home/LoginView");
                }

                                
            }
            return View();

        }

        public ActionResult UserProfileView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserProfileView(string user_name, string email_id, string phone_number, string password)
        {
            Registration update_user = new Registration();

            update_user.UserName = user_name;
            update_user.UserEmail = email_id;
            update_user.UserPhoneNumber = phone_number;
            update_user.UserPassword = password;

            //HashPasswordForStoringInConfigFile() method produces hash for the imput password passed, To hash the password Message Digest 5 (MD5) algorithm was used. 
            string hashed_password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");


            string SQLConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string SQLQuery = "Update dbo.UserDataTable set UserName = '" + user_name + "', UserEmail = '" + email_id + "', UserPhoneNumber = '" + phone_number + "', UserPassword = '" + hashed_password + "' where UserEmail ='" + Session["useremail"] + "' ;";
            DataTable GetUserDetails = Helper.GetSQLDatatable(SQLConnectionString, SQLQuery);

            //Storing the user details in session variables
            Session["username"] = update_user.UserName;
            Session["useremail"] = update_user.UserEmail;
            Session["userpassword"] = password;
            Session["userphonenumber"] = update_user.UserPhoneNumber;

            
            return View("../Registration/UserProfileView");
        }

        public ActionResult PhoneInfo()
        {
            return View();
        }

        public ActionResult PasswordInfo()
        {
            return View();
        }
    }
}