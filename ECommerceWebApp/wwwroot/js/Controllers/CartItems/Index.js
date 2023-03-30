

$(function(){
    $(".increaseBtn").each(function () {
        $(this).on("click", function () {
            var cartItemId = $(this).attr("id").substr(3)
            $.ajax({
                type: "put",
                url: `/ShoppingCart/Increase?cartItemId=${cartItemId}`,
                success: function (isSucceeded) {
                    if (isSucceeded)
                    {
                        var quantity = $("#" + cartItemId)
                        quantity.text(new Number(quantity.text()) + 1).fadeOut(0).fadeIn("slow")
                   
                        var newSubTotal = (new Number($("#prc" + cartItemId).text()) - new Number($("#dis" + cartItemId).text())) * new Number(quantity.text())
                        var oldSubTotal = new Number($("#sub" + cartItemId).text())

                        $("#sub" + cartItemId).text(newSubTotal.toLocaleString("en-us", { minimumFractionDigits: 2 })).fadeOut(0).fadeIn("slow")
                        $("#total").text((new Number($("#total").text()) - oldSubTotal + newSubTotal).toLocaleString("en-us", {minimumFractionDigits:2 })).fadeOut(0).fadeIn("slow")
                    }
                },
            })
        })
    })

    $(".decreaseBtn").each(function () {
        $(this).on("click", function () {
            var cartItemId = $(this).attr("id").substr(3)
            $.ajax({
                type: "put",
                url: `/ShoppingCart/Decrease?cartItemId=${cartItemId}`,
                success: function (isSucceeded) {
                    if (isSucceeded) {
                        var quantity = $("#" + cartItemId)

                        var oldSubTotal = new Number($("#sub" + cartItemId).text())
                        var newSubTotal = 0

                        if (new Number(quantity.text()) > 1) {
                            quantity.text(new Number(quantity.text()) - 1).fadeOut(0).fadeIn("slow")

                            newSubTotal = (new Number($("#prc" + cartItemId).text()) - new Number($("#dis" + cartItemId).text())) * new Number(quantity.text())
                            $("#sub" + cartItemId).text(newSubTotal.toLocaleString("en-us", { minimumFractionDigits: 2 })).fadeOut(0).fadeIn("slow")

                        }
                        else {
                            var cartItemsCount = $("#cartItemsCount")

                            if (new Number(cartItemsCount.text()) - 1 == 0)
                                cartItemsCount.addClass("d-none").hide(0)

                            cartItemsCount.text(new Number(cartItemsCount.text()) - 1).fadeOut(0).fadeIn("slow")
                            $("#con" + cartItemId).hide(400)
                                
                        }

                        $("#total").text((new Number($("#total").text()) - oldSubTotal + newSubTotal).toLocaleString("en-us", { minimumFractionDigits: 2 })).fadeOut(0).fadeIn("slow")

                    }
                },
            })
        })
    })

    $(".deleteBtn").each(function () {
        $(this).on("click", function () {
            var cartItemId = $(this).attr("id").substr(3)
            $.ajax({
                type:"put",
                url: `/ShoppingCart/Delete?cartItemId=${cartItemId}`,
                success: function (obj) {
                    if (obj.isSucceeded) {
                        $("#con" + cartItemId).hide("slow")

                        var subTotal = new Number($("#sub" + cartItemId).text())

                        $("#total").text((new Number($("#total").text()) - subTotal).toLocaleString("en-us", { minimumFractionDigits: 2 })).fadeOut(0).fadeIn("slow")

                        var cartItemsCount = $("#cartItemsCount")

                        if (new Number(cartItemsCount.text()) - 1 == 0)
                            cartItemsCount.addClass("d-none").hide(0)

                        cartItemsCount.text(new Number(cartItemsCount.text()) - 1).fadeOut(0).fadeIn("slow")
                    }
                    else {
                        $("#main").prepend(`<div class="alert alert-danger alert-dismissible">${obj.msg} <btn class="btn-close" data-bs-dismiss="alert"></btn></div>`).fadeOut(0).fadeIn("slow")
                    }
                },
                error: function () {
                    $("#main").prepend(`<div class="alert alert-danger alert-dismissible">Failed To Add <btn class="btn-close" data-bs-dismiss="alert"></btn></div>`).fadeOut(0).fadeIn("slow")
                }
            })
        })
    })


    $("#addressModalBtn").click(function () {
        $.ajax({
            type: "get",
            url: "/order/GetUserAddresses",
            success: function (items) {
                for (i = 0; i < items.length; ++i) {
                    $("#addressesContainer").append(`
                        <a href="order/PlaceOrder?addressId=${items[i].id}" class="list-group-item list-group-item-action d-flex border-0 shadow my-4 py-4">
                            <div class="ms-2 flex-fill">
                                <div class="row">
                                    <label class="col col-md-4  col-form-label pt-0">Country</label>
                                    <input value="${items[i].country}" class="col form-control-plaintext pt-0" disabled/>
                                </div>
                                <div class="row">
                                    <label class="col col-md-4 col-form-label">City</label>
                                    <input value="${items[i].city}" class="col form-control-plaintext" disabled/>
                                </div>
                                <div class="row">
                                    <label class="col col-md-4 col-form-label">State</label>
                                    <input value="${items[i].state}" class="col form-control-plaintext" disabled/>
                                </div>
                                <div class="row">
                                    <label asp-for="@address.StreetAddress" class="col col-md-4 col-form-label">Street Address</label>
                                    <input value="${items[i].streetAddress}" class="col form-control-plaintext" disabled/>
                                </div>
                                <div class="row">
                                    <label class="col col-md-4 col-form-label">Postal Code</label>
                                    <input value="${items[i].postalCode}" class="col form-control-plaintext" disabled/>
                                </div>
                            </div>
                        </a>
                    `)
                }
            }
        })
    })

})







