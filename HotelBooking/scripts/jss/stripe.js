
/* this js code is taken from https://stripe.com/docs/stripe-js/elements/quickstart and customised accordingly*/
// Create a Stripe client.
var stripe = Stripe('pk_test_9tq8WQOoAm76cTsVtn4guYY100HGy0s1K2');

// Create an instance of Elements.
var elements = stripe.elements();

// Create an instance of the card Element.
var card = elements.create('cardNumber', {
    classes: {
        base: "form-control",
        focus: "green",
        invalid: "error"
    }
});
var exp = elements.create('cardExpiry', {
    classes: {
        base: "form-control",
        focus: "green",
        invalid: "error"
    }
});
var cvv = elements.create('cardCvc', {
    classes: {
        base: "form-control",
        focus: "green",
        invalid: "error"
    }
});
// Add an instance of the card Element into the `card-element` <div>.
card.mount('#card-number');
exp.mount('#card-exp');
cvv.mount('#card-cvv');

// Handle real-time validation errors from the card Element.
card.addEventListener('change', function (event) {
    var displayError = document.getElementById('card-errors');
    if (event.error) {
        displayError.textContent = event.error.message;
    } else {
        displayError.textContent = '';
    }
});

// Handle form submission.
var form = document.getElementById('payment-form');
form.addEventListener('submit', function (event) {
    event.preventDefault();

    stripe.createToken(card).then(function (result) {
        if (result.error) {
            // Inform the user if there was an error.
            var errorElement = document.getElementById('card-errors');
            errorElement.textContent = result.error.message;
        } else {
            // Send the token to your server.
            stripeTokenHandler(result.token);
        }
    });
});

// Submit the form with the token ID.
function stripeTokenHandler(token) {
    // Insert the token ID into the form so it gets submitted to the server
    var form = document.getElementById('payment-form');
    var hiddenInput = document.createElement('input');
    hiddenInput.setAttribute('type', 'hidden');
    hiddenInput.setAttribute('name', 'stripeToken');
    hiddenInput.setAttribute('value', token.id);
    form.appendChild(hiddenInput);

    // Submit the form
    form.submit();
}