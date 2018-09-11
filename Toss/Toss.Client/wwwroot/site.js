function AjaxLoader() {
    var toastrId = 0;

    this.show = function (elementRef) {
        toastrId++;
        var currentToastr = toastr["info"]("", "Waiting for server response ...", { timeOut: 0 });
        currentToastr.attr("id", "toastr-" + toastrId);
        var loadingText = '<i class="fa fa-circle-o-notch fa-spin"></i> Loading...';
        if (typeof (elementRef._blazorElementRef) === "object")//null elementref are send to js as json variable instead of null
            return toastrId;
        var btn = $(elementRef);

        btn.data("original-text", $(elementRef).html());
        btn.html(loadingText);
        btn.attr("ajax-loader", toastrId);
        btn.prop("disabled", true);

        return toastrId;

    };
    this.hide = function (id) {
        toastr.clear($("#toastr-" + id));
        var btn = $("[ajax-loader=" + id + "]");
        btn.html(btn.data("original-text"));
        btn.prop("disabled", false);
    };
}

//Blazor.registerFunction("log", function (message) {
//    console.log(message);
//    return true;
//});
toastrShow = function (toastType, message) {
    toastr[toastType](message);
    return true;
};
var ajaxLoader = new AjaxLoader();
ajaxLoaderShow = function (elm) {
    return ajaxLoader.show(elm);
};
ajaxLoaderHide = function (id) {
    ajaxLoader.hide(id);
    return true;
};
showModal = function (id) {
    $("#" + id).modal("show");
    return true;
};
hideModal = function (id) {
    $("#" + id).modal("hide");
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