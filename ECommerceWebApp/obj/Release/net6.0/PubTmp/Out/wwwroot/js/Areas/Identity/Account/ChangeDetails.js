

$(function () {
    $("#changeDetails").on("click", function () {
        $.ajax({
            type: "get",
            url: "/identity/account/changeDetails",
            success: function (details) {
                $("#FirstName").val(details.firstName)
                $("#LastName").val(details.lastName)
                $("#PhoneNumber").val(details.phoneNumber)
            }
        })
    })
})




