using HotelBooking.Helpers;
using HotelBooking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace HotelBooking.Controllers
{
    public class ContactUsController : Controller
    {
        Boolean isSuccess = false;
        private object ContactDataList;

        // GET: ContactUs
        /******* Added by Neharika****
         * This method is the main control function for the functionality
         It includes, inserstion of data from form into database and generating request id and then sending that request id in email to the person who raised the query*****/
        public ActionResult ContactUs(String name, String email, String category, String message)
        {
            String viewName = "ContactUs";
            if (name !=" " && name!=null)
            { 
            System.Diagnostics.Debug.WriteLine("Parameters___neharika" + name + "" + email + "" + category + "" + message);

                // Connection string and SQL query are required to connect and get data from database
                string SQLConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
    
            string SQLQuery = "  insert into ContactUsInfo(UserName, EmailID, Categories, Comment) values('" + name + "'" + ",'" + email + "'" + ",'" + category + "'," + "'" + message + "')";

           System.Diagnostics.Debug.WriteLine(SQLQuery);
            //The insertContact Info method is to insert the data from the form into the database
            Boolean InsertContactUsData = Helper.insertContactInfo(SQLConnectionString, SQLQuery);
            if (InsertContactUsData)
            {
                string getQuery = "select * from dbo.ContactUsInfo where UserName='" + name+"' and EmailID='"+email+"' and Categories='"+category+"' and Comment='"+message+"'";
                    System.Diagnostics.Debug.WriteLine(getQuery);
                    DataTable GetContactData = Helper.GetSQLDatatable(SQLConnectionString, getQuery);

                //convert datatable to list as return type of method is in List
               
                List<ContactUs> ContactDataList = new List<ContactUs>();

                foreach (DataRow row in GetContactData.Rows)
                {
                    ContactUs ContactObj = new ContactUs();
                        ContactObj.RequestId = (int)row["RequestId"];
                        System.Diagnostics.Debug.WriteLine("Request ID" + ContactObj.RequestId);
                    ContactDataList.Add(ContactObj);

                        //sendEmailToUser is the method for sending email on successfully generation of the request id
                        try
                        {
                         Boolean emailSent = sendEmailToUser( name,email, category,message, ContactObj.RequestId);
                            if (emailSent == true)
                                isSuccess = true;
                            else
                            { 
                                isSuccess = false;
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Problem in sending email" + e);
                        }
                        //Set the view name as per success or error of the method
                        if(isSuccess==true)
                        {
                        viewName = "SuccessContactQuery";
                        ViewBag.RequestId= ContactObj.RequestId;
                        }
                        else
                            viewName = "ErrorContactQuery";
                    }
            }
            }
            
            return View(viewName);
        }

        /*Method created for sending email after successful insertion of the data in the database*/

        public Boolean sendEmailToUser(String name, String email, String category, String message, int requestId)
        {
            Boolean mailSent = false;
            try
            {
                String senderEmail = System.Configuration.ConfigurationManager.AppSettings["senderEmail"].ToString();
                String senderPassword = System.Configuration.ConfigurationManager.AppSettings["senderPassword"].ToString();
                // String emailBody = "Hi " + name + "," + Environment.NewLine + "Your request number " + requestId + " has been raised." + Environment.NewLine + "We are looking into it and will get back to you in 24 hours " + "Thanks</br> Support Team";
                String subject = "Your request has been raised";
                String emailBody = "";
                SmtpClient client = new SmtpClient("smtp.gmail.com",587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail,senderPassword);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Hi "+name+",");
                sb.AppendLine("");
                sb.AppendLine("Your request ID is " + requestId + ". We are looking into it and will get back to you in 24 hours.");
                sb.AppendLine("");
                sb.AppendLine("Thanks");
                sb.AppendLine("Support Team");
                emailBody = sb.ToString().Replace(Environment.NewLine, "<br />");

                MailMessage mailMessage = new MailMessage(senderEmail, email, subject, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);
                mailSent = true;

            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception"+e);
                mailSent = false;
            }
            return mailSent;
        }
    }
}