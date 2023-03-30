

$(function () {
    $(".cancelBtn").each(function () {
        $(this).click(function () {
            var orderId = $(this).attr("id").substr(3)
            $.ajax({
                type:"put",
                url: `/Order/Cancel?orderId=${orderId}`,
                success: function (isSucceeded) {
                    if (isSucceeded) {
                        $("#sts" + orderId).empty().text("Canceled").fadeOut(0).fadeIn("slow")
                        $("#cle" + orderId).hide("slow")
                        $("#del" + orderId).removeClass("d-none").hide(0).show("slow")
                    }
                    else {
                        $("#main").prepend(`<div class="alert alert-danger alert-dismissible">${obj.msg} <btn class="btn-close" data-bs-dismiss="alert"></btn></div>`).fadeOut(0).fadeIn("slow")
                    }
                }
            })
        })
    })

    $(".deleteBtn").each(function () {
        $(this).click(function () {
            var orderId = $(this).attr("id").substr(3)
            $.ajax({
                type: "put",
                url: `/Order/Delete?orderId=${orderId}`,
                success: function (isSucceeded) {
                    if (isSucceeded) {
                        $("#con" + orderId).hide("slow")
                    }
                    else {
                        $("#main").prepend(`<div class="alert alert-danger alert-dismissible">${obj.msg} <btn class="btn-close" data-bs-dismiss="alert"></btn></div>`).fadeOut(0).fadeIn("slow")
                    }
                }
            })
        })
    })
})








