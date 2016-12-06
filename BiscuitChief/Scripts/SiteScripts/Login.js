
$(document).ready()
{
    //Login method
    $("#btnSubmit").click(Login);
}

function Login() {
    //Make sure the form is valid
    if ($("form").valid()) {
        var username = $("#UserName").val();
        var password = $("#Password").val();
        var returnurl =
        $.ajax({
            method: "POST",
            url: "/api/login/",
            data: { UserName: username, Password: password },
            headers: { '__RequestVerificationToken': $("input[name=__RequestVerificationToken]").val() },
            success: function (data) {
                var returnurl = getParameterByName("ReturnUrl");
                if (returnurl == "" || returnurl == null) {
                    returnurl = "/";
                }
                window.location = returnurl;
            },
            error: function (xhr, textStatus, errorThrown) {
                alert(xhr.responseText);
            }
        });
    }
}