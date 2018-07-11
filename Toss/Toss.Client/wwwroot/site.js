function AjaxLoader() {
    var toastrId = 0;
    
    this.show = function (elementRef) {
        toastrId++;
        var currentToastr = toastr["info"]("","Waiting for server response ...", {timeOut:0});
        currentToastr.attr("id", "toastr-" + toastrId);
        var loadingText = '<i class="fa fa-circle-o-notch fa-spin"></i> Loading...';
        var btn = $(elementRef);
        btn.data("original-text", $(elementRef).html());
        btn.html(loadingText);
        btn.attr("ajax-loader", toastrId);
        btn.prop( "disabled", true );
        return toastrId;
        
    };
    this.hide = function (id) {
        toastr.clear($("#toastr-" + id));
        var btn = $("[ajax-loader=" + id + "]");
        btn.html(btn.data("original-text"));
        btn.prop( "disabled", false );
    };
}

Blazor.registerFunction("log", function (message) {
    console.log(message);
    return true;
});
Blazor.registerFunction("toastr", function (toastType, message) {
    toastr[toastType](message);
    return true;
});
var ajaxLoader = new AjaxLoader();
Blazor.registerFunction("ajaxLoaderShow", function (elm) {
    return ajaxLoader.show(elm);
});
Blazor.registerFunction("ajaxLoaderHide", function (id) {
    ajaxLoader.hide(id);
    return true;
});
Blazor.registerFunction("showModal", function (id) {
   $("#"+id).modal("show");
    return true;
});

const readUploadedFileAsText = (inputFile) => {
    const temporaryFileReader = new FileReader();
    return new Promise((resolve, reject) => {
        temporaryFileReader.onerror = () => {
            temporaryFileReader.abort();
            reject(new DOMException("Problem parsing input file."));
        };

        temporaryFileReader.onload = (e) => {
            var arrayBuffer = e.target.result;
            var array = new Uint8Array(arrayBuffer);         
            var data = { content: btoa(array) };
            resolve(data);
        };
        console.log(inputFile.files[0]);
        temporaryFileReader.readAsArrayBuffer(inputFile.files[0]);
    });
};
Blazor.registerFunction("getFileData", function (inputFile) {
    var expr = "#" + inputFile.replace(/"/g, '');
    return readUploadedFileAsText($(expr)[0]);
});