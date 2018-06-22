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