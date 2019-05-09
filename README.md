ASP .NET MVC Hotel Booking project
----------------------------------
Authors : 
- Jigar Joshi, B00812722
- Jismy Johnson, B00813344
- Abraham Vineel Katikala, B00793815
- Dinesh Kuncham, B00801162
- Neharika Sehgal, B00806887
        

Project & Feature Description
----------------------------------
This is a hotel booking web application developed using ASP.NET MVC framework.
Below are the implemented features for this project.
*  Search Hotels Feature
    1. This page can be navigated from Home page.
	2. When user enters City name, check-in date, check-out date, number of adults, and number of children, It will show the relevant hotels on hotel search page.
    3. Validation has been applied on home page as well as Search page. Validation will be fired in below cases on home page.
        - Wrong date format.
    	- When user tries to enter alphabetic values in date fields.
    	- When City Name, Check-in date, and check-out date are blank.
    	- Check-in date or Check-out date less than today's date.
    	- Check-in date is greater than check-out date.
	4. Once user enters valid inputs on home page, page will be redirected to search page.
	5. Searh page also has search bar to change Hotel name, check-in, check-out date. Same validation will fire as mentioned in third point.
    6. User can use filters to refine the results. Below mentioned filters have been provided. User can only use one filter at a time.
		- Price range
		- Property Type
		- Star Rating
	7. User can also clear filters by clicking on clear button on search page
    8. All the data is fetched from SQL Server.
	9. As of now two cities (halifax, toronto) have been added in database.
   10. User can add entries to HotelMaster database. It will reflect on hotel search page. 

*  Reviews Feature
    1. Display all reviews of selected hotel.
	2. Let user provide review for a hotel.
	3. Once the review is provided by user, they cannot submit another review.
	4. Display average rate of a hotel

*  Wishlist Feature
    1. User can wishlist their desired hotels. This feature is accessible only after user logg in to the web-application.
    2. User can wishlist a hotel by clicking on the wishlist icon. If the user is not logged in, the application will redirect to log in page.
    3. User can also remove the wishlist by clicking the same icon.
    4. User can view all the wishlisted hotel information in the user drop down menu.

*  Payment Feature
    1. When user clicks on proceed to payment they are redirected to payment page.
    2. User enters either debit or credit card details.
    3. If the sufficient amount is available in the card, amount is deducted from the card and room is booked under the user for their desired hotel.
    4. If the payment is unsuccessful,then the reason for unsuccessful payment is shown to user and he can try again.

*  Booking Feature
    1. When user clicks on Book Now from hotel description page they are redirected to booking page
    2. User details such as name, mobile, and mail are auto popluated from the user details.
    3. User can change the ChekcIn and CheckOut dates.
    4. User can change the number of rooms based on the number of adults and child.
    5. Total price including tax is shown to user.
    6. If user satisifes with all the details he can pay the amount using Payment feature.
    7. After successful booking, user can check the booking details from booking history page.
    7. From booking history page, user can modify the checkout date one day before checkIn .
    8. From booking history page, user can cancel the booking only one day before checkIn.
    9. From booking history page, user can update the mobile and mail or change the number of rooms for the booking.
    10. From booking history page, if the user want to add more rooms or extend the stay he can check the price and using payment feature user can pay amount for additional room or days.

* User Profile Management Feature
    1. The user clicks on sign-up and enters the required details such as username, email address, phone number, password and confirm password.
    2. The user is notified if any details are missing or if any invalid details are entered.
    3. The user clicks on create account button and the user is redirected to the login page.
    4. The user enters the login credentials and clicks on log-in.
    5. The user can view the user details in profile settings page which can be selected from the drop down under the name of the logged-in user.
    6. The user modifies the entered details and clicks on update.
    7. The user notified when the details are updated.

* Trending Feature
    1. On the homepage the user can view the list of trending hotels.
    2. From the list of trending hotels the user can select the desired hotel by clicking on the know more button.
    3. The user is redirected to the hotel description page of the selected hotel.
	
* Contact Us Feature
    1. User selects the Contact Us Page
	2. User fills the form with details such as name, email address, message.
	3. User clicks on 'Submit button' 
	4. A request Id will be generated and success message along with request id is shown on the screen
	5. User will also receive an email with the request Id

* Share On Facebook Feature	
    1. User logs in with valid credentials 
	2. Search the hotel with criteria as per the requirement and check availability of the hotels.
	3. User browses throught the hotel and can share the hotel on the facebook page
	
    
Why .NET MVC?	  
---------------------------------------
It divides data, business logic, and user interface which enables faster development of an application.

The different layout pages have been used which could work as a master design for the pages. It reduces redundancies of the code.

### Installations

* Download Microsoft Visual Studio. Link for [download](https://download.cnet.com/Visual-Studio-Professional-2015/3000-2212_4-)
* Download SQL Server Download [here](https://www.microsoft.com/en-ca/sql-server/sql-server-downloads)
* Download SQL Server Management Studio 17. Link for [download](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017)

## Built With

* [Visual Studio 2015](https://docs.microsoft.com/en-us/visualstudio/welcome-to-visual-studio-2015?view=vs-2015) - The web framework used for backend functionality, implemented in .net MVC framewrk.
* [Bootstrap v3.4.0](https://getbootstrap.com/docs/3.4/) - Front-end framework
* [Jquery UI 1.10.3](https://api.jqueryui.com/1.10/) - Used for front end designing ad validations.
* [HTML5](https://developer.mozilla.org/en-US/docs/Web/Guide/HTML/HTML5) - For form validations.
* [Font Awesome v4.7.0](https://fontawesome.com/v5.8.1/get-started/)- For star rating icons.


Steps to run
---------------------------------------
To run this application, user must install Visual Studio 2015 which can be downloaded from its official website https://visualstudio.microsoft.com/vs/older-downloads/.

It aslo requires SQL server to run this application. SQL server 2017 is preferred version.

Database backup file will be provided in zip folder.

Steps to upload SQL database in local SQL server.
 1. Open SQL Server Management Studio
 2. Connect to local server
 3. Right click on Databases folder and select restore database
 4. From the pop-up, give local SQl database backup path
 5. Click on Ok button to restore
 6. Get Connection string of the database restored
 7. In the payment feature, To test the success case enter any name, card number as 4242424242424242 , any expiry date and cvv and click Pay Securely button(For testing purposes stripe has some pre-defined test card numbers)[9].
 8. To test failure case enter vard number as 4000000000000002 , any expiry date and cvv and click Pay Securely button [9].


 For further information, visit the site https://www.stellarinfo.com/article/restore-sql-server-database-from-bak-file.php

 Now, Connection string needs to be added in Project's web.config file under CONFIGURATION --> APPSETTING.
 Replace value (Local Database Connection String) of key "ConnectionString".
 
 ```
<configuration>
  <appSettings>
    <add key="webpages:Version" value="3.0.0.0"/>
    <add key="webpages:Enabled" value="false"/>
    <add key="ClientValidationEnabled" value="true"/>
    <add key="UnobtrusiveJavaScriptEnabled" value="true"/>
    <add key="ConnectionString" value="Data Source=DESKTOP-T5KMH44;Initial Catalog=HotelBuddy;Integrated Security=True" />
  </appSettings>
```

Steps required to access the site.
 1. Open application solution from visual studio.
 2. From the Solution Explorer, open HomeController.cs from Controllers folder.
 3. From the Standard toolbar, run the application
 4. By default, home page will be opened. 



Design Choices
----------------------------------------
To improve the look and feel of design, bootstrap, Jquey, font-awesome have been used.

Bootstrap is responsible for making a website responsive. It is also used for icons and colour scheme of an entire application.

For validation JQuery and .Net MVC classes have been used.

.Net MVC partial views have been used for reusability of the code.


Feature code file locations
-----------------------------------------
The code is written in .Net MVC and it follows model, view and controller(such as SearchHotelsController, HomeController, ContactUs Controller) structure for the code.
Views folder: It hasas all web pages which are displayed as an interface.
Model folder: It has all files related to database access.
Controller folder. It contains all logic to fetch data from the database and display on the view.
Helper folder: It is created to add common methods required to fetch/insert(or any database related operations) data from database.
Hierarchy of the code structure has been maintained.
All Customised CSS Scripts has been kept in the Content folder
All Bootstrap related scripts are created under Scripts folder
All Images related to the pages are kept under Images folder
Generic variables like connetion string, email id has been placed in Web.config for making it configurable


Problems faced /Customisation
--------------------------------------
1)While implementing the logic for the ContactUs Page, requestID should have been automatically generated at the database level and then retrieved and displayed for the users.
Request ID was created an auto increment field in the table and retrieved in the code. Code snippet as shown below:
 ```
 foreach (DataRow row in GetContactData.Rows)
                {
                    ContactUs ContactObj = new ContactUs();
                        ContactObj.RequestId = (int)row["RequestId"];
                        System.Diagnostics.Debug.WriteLine("Request ID" + ContactObj.RequestId);
                    ContactDataList.Add(ContactObj);
				}
 ```				
2) For sending emails from gmail id, there was a problem which states "Gmail Error :The SMTP server requires a secure connection or the client was not authenticated. The server response was: 5.5.1 Authentication Required". This problem was resolved by following [6] . After year 2016, an additional permission is required by gmail to allow it to send email by external application.

Note: If you change email id in Web.config , follow [6],[7] to enable mail sending

3) To display two different views from same controller method according to processing status of the method,view name can be directly returened (return View(viewName));

4) There was a problem faced while sending data from controller to the view.
View Bag was used to send the requestID to the view.[8]. Below is the code snippet
 ViewBag.RequestId= ContactObj.RequestId;
 
5)Backup for the database to be provided with code was not successful. Permission setting on the folder level was modified to create .bak file successfully[9]

6)Creation of email body: Break lines were not working. Stringbuilder object is required to be created for the code [10] . Below is the snippet.
 ```
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Hi "+name+",");
                sb.AppendLine("");
                sb.AppendLine("Your request ID is " + requestId + "  We are looking into it and will get back to you in 24 hours.");
                sb.AppendLine("");
                sb.AppendLine("Thanks");
                sb.AppendLine("Support Team");
                emailBody = sb.ToString().Replace(Environment.NewLine, "<br />");
 ```				
7) During build of the project, there are some errors faced  due to improper loading of System.Web.MVC assembly references, follow [13] to resolve those errors and rebuild the solution.

8) While integrating the stripe we encountered an  'System.IO.FileLoadException', after nearly spending 3 hours we found that it is because of the package version difference between stripe and  Newtonsoft.Json.dll
   after updating the Newtonsoft to 12.0.1 the issue resolved.

Git lab Link
------------
Link for the working repository is [here](https://git.cs.dal.ca/jjoshi/hotelbuddy.git)

Open it and click on download zip option to download solution.


References
-----------------------------------------

1. a. Mark Otto, "Navbar", Getbootstrap.com, 2019. [Online]. Available: https://getbootstrap.com/docs/4.1/components/navbar/. [Accessed: 09- Apr- 2019].
2. "Bootstrap Footer - examples & tutorial. Basic & advanced usage", Material Design for Bootstrap, 2019. [Online]. Available: https://mdbootstrap.com/docs/jquery/navigation/footer/.         [Accessed: 09- Apr- 2019].
3. Toast Message: The toast message displayed when a review is submitted is referred from [Codepen](https://codepen.io/kipp0/pen/pPNrrj).
4. Star Rating: The star rating for providing rating is referred from [Codepen](https://codepen.io/ashdurham/pen/HBxLK).
5. For removing wishlist s referred from [Davepaquette](https://www.davepaquette.com/archive/2014/02/24/simple-delete-confirmation-in-asp-net-mvc.aspx).
6. G. Required, R. Parmar and B. Rosenfeld, "Gmail Error :The SMTP server requires a secure connection or the client was not authenticated. The server response was: 5.5.1 Authentication Required", Stack Overflow, 2019. [Online]. Available: https://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not. [Accessed: 23- Mar- 2019]
7. "Let less secure apps access your account - Google Account Help", Support.google.com, 2019. [Online]. Available: https://support.google.com/accounts/answer/6010255?hl=en. [Accessed: 23- Mar- 2019]
8. D. MVC4, A. V, A. V, E. Sajjad and J. Rabadiya, "Display List return from a controller to a table in view ASP.NET MVC4", Stack Overflow, 2019. [Online]. Available: https://stackoverflow.com/questions/28558157/display-list-return-from-a-controller-to-a-table-in-view-asp-net-mvc4. [Accessed: 23- Mar- 2019]
9. "Creating Charges | Stripe Payments", Stripe.com, 2019. [Online]. Available: https://stripe.com/docs/charges. [Accessed:  09- Apr- 2019].

Images 
-----------------------------------------

1. Canadavisa.com, 2019. [Online]. Available: https://www.canadavisa.com/images/Optimized-Alberta-Morraine-lake.jpg. [Accessed: 09- Apr- 2019].
2. Media-cdn.tripadvisor.com, 2019. [Online]. Available: https://media-cdn.tripadvisor.com/media/photo-s/07/0a/37/d0/atlantica-hotel-halifax.jpg. [Accessed: 09- Apr- 2019].
3. Media.istockphoto.com, 2019. [Online]. Available: https://media.istockphoto.com/photos/halifax-waterfront-aerial-view-picture-id486845192?   k=6&m=486845192&s=612x612&w=0&h=8bwbfguNqeyc2eAZR3oDzJix2KtavbLGP4ifzz_t-MA=. [Accessed: 09- Apr- 2019].

