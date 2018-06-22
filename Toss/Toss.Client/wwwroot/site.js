function AjaxLoader() {
    var toastrId = 0;
    
    this.show = function () {
        toastrId++
        var currentToastr = toastr["info"]("Waiting for server response ...", {timeOut:100000});
        currentToastr.attr("id","toastr-"+toastrId)
        return toastrId;
        
    };
    this.hide = function (id) {
        console.log(id);
        toastr.clear($("#toastr-"+id));
    }
}

Blazor.registerFunction('log', function (message) {
    console.log(message);
    return true;
});
Blazor.registerFunction('toastr', function (toastType, message) {
    toastr[toastType](message)
    return true;
});
var ajaxLoader = new AjaxLoader();
Blazor.registerFunction('ajaxLoaderShow', function () {
    return ajaxLoader.show();
});
Blazor.registerFunction('ajaxLoaderHide', function (id) {
    ajaxLoader.hide(id);
    return true;
});