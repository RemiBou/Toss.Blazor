showModal = function (elementRef, closeCallBack) {
    $(elementRef).modal("show");
    if (closeCallBack) {
        $(elementRef).on('hidden.bs.modal', function (e) {
            closeCallBack.invokeMethodAsync('OnClose');
            $(elementRef).off('hidden.bs.modal');//we unsubscribe the event as we won't need it anymore
        });
    }
    return true;
};
hideModal = function (elementRef) {
    $(elementRef).modal("hide");
    return true;
};

const readUploadedFileAsText = (inputFile) => {
    const temporaryFileReader = new FileReader();
    return new Promise((resolve, reject) => {
        temporaryFileReader.onerror = () => {
            temporaryFileReader.abort();
            reject(new DOMException("Problem parsing input file."));
        };
        temporaryFileReader.addEventListener("load", function () {
            console.log("JS : file read done");
            resolve(temporaryFileReader.result.split(',')[1]);
        }, false);
        temporaryFileReader.readAsDataURL(inputFile.files[0]);
    });
};
getFileData = function (inputFile) {

    return readUploadedFileAsText(inputFile);
};

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

XMLHttpRequest.prototype.open_before = XMLHttpRequest.prototype.open;

XMLHttpRequest.prototype.open = function (method, url, async) {
    if (url.endsWith(".dll")) {
        url = url.replace("dll", "blazor");
    }
    return this.open_before(method, url, async);
};
runCaptcha = function (actionName) {
    return new Promise((resolve, reject) => {
        grecaptcha.ready(function () {
            grecaptcha.execute('6LcySnsUAAAAAKFZn_ve4gTT5kr71EXVkQ_QsGot', { action: actionName })
                .then(function (token) {
                    resolve(token);
                });
        });

    });
};