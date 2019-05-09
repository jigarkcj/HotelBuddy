

window.onload = function startRedirect() {
    //wait for 5 seconds on loading the page to redirect
    setTimeout(redirect, 5000)
};
function redirect() {
    // Chnage your redirection link here
    var getUrl = window.location;
    //get the base url from the browser ex: http://localhost:50783/
    var baseUrl = getUrl.protocol + "//" + getUrl.host + "/" + getUrl.pathname.split('/')[0];
    var url = "";
    if (document.getElementById('status').innerHTML == "Successful") {
        url = baseUrl + "Booking/BookedDetails";
    } else if (document.getElementById('status').innerHTML == "Unsuccessful") {
        url = baseUrl + "Home";
    }
    //redirect to url
    window.location = url
}