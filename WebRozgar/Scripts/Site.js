/// <reference path="jquery-1.8.2.js" />
/// <reference path="bootstrap.js" />
$(document).ready(function () {
    $('#searchresult').hide();
    $('#search').focus();

    $('#search').keypress(function (e) {
        $('#search').animate({
            width: '400px'
        }, 500, function () { });
        $('#searchresult').show();
        e.stopPropagation();
    });
    $(document).click(function () {
        $('#search').animate({
            width: '200px'
        }, 500, function () { });
        $('#searchresult').hide();
    });
    $('.loadingindicator').hide();
    $('#aloadingindicator').hide();

   
});
function AjaxStart() {
    $('.loadingindicator').show();
    $('#buttonsubmit').attr('disabled', 'disabled');
}
function AjaxStop() {
    $('.loadingindicator').hide();
}

function ApplySuccess(e) {
    $(e).val('Applied');
    $(e).removeClass("btn-primary").addClass("btn-success");
}