using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBooking.Helpers;
using System.Data;
using System.Web.UI;

namespace HotelBooking.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
          
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email_id, string password)
        {
            string PreviousURL = "";
            if (Session["PreviousURL"] != null) { 
             PreviousURL = Session["PreviousURL"].ToString();
            }
            DataTable UserData =Helper.CheckValidUser(email_id, password);

            if (!UserData.Rows.Count.Equals(0))
            {
                DataRow row = UserData.Rows[0];
                Session["UserID"] = row["UserID"].ToString();
                Session["UserName"] = row["UserName"].ToString();
                Session["UserEmail"] = row["UserEmail"].ToString();
                Session["UserPhoneNumber"] = row["UserPhoneNumber"].ToString();
                Session["UserPassword"] = password;
                if (PreviousURL.Contains("HotelDescription"))
                {
                    return RedirectToAction("HotelBooking", "Booking");
                }
             

                return RedirectToAction("HotelBookingView", "Home");

            }
           // TempData["InvalidCredentials"] = "<script>alert('Invalid email or password. Please enter again.');</script>";
            TempData["InvalidCredentials"] = "Invaild";
            return RedirectToAction("LoginView", "Home");
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("HotelBookingView", "Home");
        }
    }
}