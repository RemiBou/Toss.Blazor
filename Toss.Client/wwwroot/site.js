

getDocumentCookie = function () {
    return Promise.resolve(document.cookie);
};

navigatorLanguages = function () {
    return Promise.resolve(navigator.languages);
};

stripeCheckout = function (callBackInstance, amount) {
    var handler = StripeCheckout.configure({
        key: 'pk_test_IAEerhZ6JVmmcj9756zIZegI',
        image: 'https://stripe.com/img/documentation/checkout/marketplace.png',
        locale: 'auto',
        token: function (token) {
            callBackInstance.invokeMethodAsync('TokenReceived', token.id)
                .then(r => console.log(token));
        },
        currency: 'EUR'
    });
    // Open Checkout with further options:
    handler.open({
        name: 'Stripe.com',
        description: '2 widgets',
        zipCode: true,
        amount: amount
    });
    return Promise.resolve();
};
disableCaptcha = false;
runCaptcha = function (actionName) {
    if (disableCaptcha)
        return Promise.resolve(actionName);
    return new Promise((resolve, reject) => {
        grecaptcha.ready(function () {
            grecaptcha.execute('6LcySnsUAAAAAKFZn_ve4gTT5kr71EXVkQ_QsGot', { action: actionName })
                .then(function (token) {
                    resolve(token);
                });
        });

    });
};