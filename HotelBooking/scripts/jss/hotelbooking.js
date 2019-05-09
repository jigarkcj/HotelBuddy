
document.getElementById("adults").disabled = true;
document.getElementById("child").disabled = true;
var bookedPrice;

//sets the price of the table when page loaded it is used to check with the changed price and calculate remaining amount
function setBookedPrice(price) {
    bookedPrice = document.getElementById("totalPrice").innerText;
}

// increases the value of number of adults
function adultCountPlus() {
    var adultValue = parseInt(document.getElementById("adults").value);
    var roomsValue = parseInt(document.getElementById("rooms").value);
    var incrementValue = increment(adultValue);
    if (Math.ceil(incrementValue / 2) <= roomsValue) {
        document.getElementById("adults").value = incrementValue;
        document.getElementById("adultsHidden").value = incrementValue;
        var adultTtext = incrementValue > 1 ? incrementValue + " Adults" : incrementValue + " Adult";
        document.getElementById("adultCount-description").innerText = adultTtext;
    } else {
        document.getElementById("adults").value = adultValue;
        document.getElementById("adultsHidden").value = adultValue;
        countAlert("Adults");
    }
}
// decreases the value of number of adults
function adultCountMinus() {
    var adultValue = document.getElementById("adults").value;
    var decrementValue = decrement(adultValue);
    //To make sure number of adults is not zero
    decrementValue = decrementValue == 0 ? 1 : decrementValue;
    document.getElementById("adults").value = decrementValue;
    document.getElementById("adultsHidden").value = decrementValue;
    var adultTtext = decrementValue > 1 ? decrementValue + " Adults" : decrementValue + " Adult";
    document.getElementById("adultCount-description").innerText = adultTtext;
}

// increases the value of number of child
function childCountPlus() {
    var childValue = document.getElementById("child").value;
    var incrementValue = increment(childValue);
    var roomsValue = parseInt(document.getElementById("rooms").value);
    if (Math.ceil(incrementValue / 2) <= roomsValue) {
        document.getElementById("child").value = incrementValue;
        document.getElementById("childHidden").value = incrementValue;
        var text = "," + incrementValue + " Child";
        document.getElementById("childCount-description").innerText = text;
    } else {
        document.getElementById("child").value = childValue;
        document.getElementById("childHidden").value = childValue;
        countAlert("child");
    }
}

// decreases the value of number of child
function childCountMinus() {
    var childValue = document.getElementById("child").value;
    var decrementValue = decrement(childValue);
    document.getElementById("child").value = decrementValue;
    document.getElementById("childHidden").value = decrementValue;
    if (parseInt(decrementValue) <= 0) {
        document.getElementById("childCount-description").innerText = "";
    } else {
        var text = "," + decrementValue + " Child";
        document.getElementById("childCount-description").innerText = text;
    }
}

function increment(value) {
    return parseInt(value) + 1
}

function decrement(value) {
    if (parseInt(value) > 1) {
        return parseInt(value) - 1
    } else {
        return 0;
    }
}

//change the value of room and adults and child
function roomChange(selectedValue, price) {
    var selectedValue = parseInt(selectedValue);
    var text = selectedValue > 1 ? selectedValue + " Rooms" : selectedValue + " Room";
    document.getElementById("number-of-rooms").innerText = text;
    document.getElementById("rooms").value = selectedValue + "";
    var adultValue = parseInt(document.getElementById("adults").value);
    var childValue = parseInt(document.getElementById("child").value);
    if (Math.ceil(adultValue / 2) > selectedValue) {
        var newAdultValue = selectedValue * 2;
        document.getElementById("adults").value = newAdultValue;
        document.getElementById("adultsHidden").value = newAdultValue;
        var adultTtext = newAdultValue > 1 ? newAdultValue + " Adults" : newAdultValue + " Adult";
        document.getElementById("adultCount-description").innerText = adultTtext;
    }
    if (Math.ceil(childValue / 2) > selectedValue) {
        var newChildValue = selectedValue * 2;
        document.getElementById("child").value = newChildValue;
        document.getElementById("childHidden").value = newChildValue;
        var text = "," + newChildValue + " Child";
        document.getElementById("childCount-description").innerText = text;
    }
    //changing price in the table based on rooms
    priceTableUpdate(price, selectedValue);

}

function countAlert(type) {
    alert("Only 2 " + type + " per Room");
}

//updates the price table based on the selected room value and checkin and chekout
function priceTableUpdate(price, roomValue) {
    var days = DateDiff(document.getElementById("checkin-date").value, document.getElementById("checkout-date").value)
    days = isNaN(days) ? 0 : days;
    document.getElementById("basePrice").innerText = price * roomValue * days;
    document.getElementById("basePriceHidden").value = price * roomValue * days;
    document.getElementById("taxes").innerText = price * roomValue * days * 0.15;
    document.getElementById("totalPrice").innerText = price * roomValue * days + (price * roomValue * days * 0.15);
    document.getElementById("totalPriceHidden").value = price * roomValue * days + (price * roomValue * days * 0.15);
    try {
        if (parseFloat(bookedPrice) < parseFloat(document.getElementById("totalPrice").innerText)) {
            document.getElementById("already-paid-message").innerText = "You already paid $" + bookedPrice + " ,Pay $";
            document.getElementById("remaining-amount").innerText = (parseFloat(document.getElementById("totalPrice").innerText) - parseFloat(bookedPrice));
            document.getElementById("totalPriceHidden").value = document.getElementById("remaining-amount").innerText;
            document.getElementById("remaining-amount-message").innerText = " more to book";
            document.getElementById("modify-button").innerText = "Proceed to Payment";
            document.getElementById("modify-form-action").action = "HotelPayment";
        } else {
            document.getElementById("modify-button").innerText = "Update";
            document.getElementById("modify-form-action").action = "ModifyUserDetails";
            document.getElementById("already-paid-message").innerText = "";
            document.getElementById("remaining-amount").innerText = "";
            document.getElementById("remaining-amount-message").innerText = "";
        }
    } catch (err) {

    }
}

//updates the price from date alone
function priceTableUpdateFromDate(price) {
    var checkIn = new Date(document.getElementById("checkin-date").value);
    var checkOut = new Date(document.getElementById("checkout-date").value);
    if (checkIn >= checkOut) {
        alert("Please ensure that the Check Out Date is greater than the Checkin Date.");
        document.getElementById("checkout-date").value = null;
        priceTableUpdate(0, 0);

    } else {
        var roomsValue = parseInt(document.getElementById("rooms").value);
        document.getElementById("check-in-time").innerText = document.getElementById("checkin-date").value;
        document.getElementById("check-out-time").innerText = document.getElementById("checkout-date").value;
        priceTableUpdate(price, roomsValue);
    }
}
function check(price) {
    var roomsValue = parseInt(document.getElementById("rooms").value);
    priceTableUpdate(price, roomsValue);
}

//updates the price after loading the page
function priceTableUpdateAtFirst(price) {
    document.getElementById("basePrice").innerText = price;
    document.getElementById("basePriceHidden").value = price;
    document.getElementById("taxes").innerText = price * 0.15;
    document.getElementById("totalPrice").innerText = price + (price * 0.15);
    document.getElementById("totalPriceHidden").value = price + (price * 0.15);
}

function DateDiff(startDateString, endDateString) {
    const startDate = new Date(startDateString);
    const endDate = new Date(endDateString);
    const diffTime = Math.abs(endDate.getTime() - startDate.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
}