



$(function () {
    $(".delAddressBtn").each(function () {
        $(this).click(function () {
            var addressId = $(this).attr("id")
            $.ajax({
                type:"put",
                url: `/Identity/Account/DeleteAddress?addressId=${addressId}`,
                success: function (isSucceeded) {
                    if (isSucceeded) {
                        $("#con" + addressId).remove()
                        $("#main").fadeOut(0).fadeIn("slow")
                    }
                    else 
                        $("#main").prepend(`<div class="alert alert-danger alert-dismissible">Failed To Delete <btn class="btn-close" data-bs-dismiss="alert"></btn></div>`).fadeOut(0).fadeIn("slow")
                    
                },
                error: function () {
                    $("#main").prepend(`<div class="alert alert-danger alert-dismissible">Failed To Add <btn class="btn-close" data-bs-dismiss="alert"></btn></div>`).fadeOut(0).fadeIn("slow")
                }
            })
        })
    })
})





