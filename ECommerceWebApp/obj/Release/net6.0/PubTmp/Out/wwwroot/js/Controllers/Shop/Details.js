
$(function () {

    $("#options").removeClass("d-none").fadeOut(0)
    $("#reviews").removeClass("d-none").fadeOut(0)

    $("#publicBtn").on("click", function () {
        $("#public").fadeOut(0).fadeIn(1000)
        $("#options").fadeOut(0)
        $("#reviews").fadeOut(0)

        $(this).addClass("active")
        $("#optionsBtn").removeClass("active")
        $("#reviewsBtn").removeClass("active")
    })

    $("#optionsBtn").on("click", function () {
        $("#public").fadeOut(0)
        $("#options").fadeOut(0).fadeIn(1000)
        $("#reviews").fadeOut(0)

        $(this).addClass("active")
        $("#publicBtn").removeClass("active")
        $("#reviewsBtn").removeClass("active")
    })

    const takeReviews = 6
    var skipReviews = 0

    function loadReviews() {
        $.ajax({
            type: "get",
            url: `/Review/GetItemReviews?itemId=${$("#itemId").val()}&skip=${skipReviews}&take=${takeReviews}`,
            success: function (items) {
                for (i = 0; i < items.length; ++i) {
                    $("#reviews").append(`
                     <div class="d-flex my-3">
                         <div class="me-2">
                             <img src="/${items[i].imgUrl}" class=" rounded-circle" style="width:46px;height:46px;"/>
                         </div>
                         <div class="ms-2">
                             <h6 class="text-primary mb-1">${items[i].name}</h6>
                             <p class="small">${items[i].comment}</p>
                         </div>
                     </div>
                    `)
                }

                if (items.length < takeReviews)
                    $("#showMoreReviewsCon").fadeOut(0)
                else
                    $("#showMoreReviewsCon").fadeOut(0).fadeIn("slow")

                skipReviews += takeReviews
            }
        })
    }

    $("#showMoreReviews").click(function () {
        loadReviews()
    })

    $("#reviewsBtn").on("click", function () {
        $("#public").fadeOut(0)
        $("#options").fadeOut(0)
        $("#reviews").fadeOut(0).fadeIn(1000)

        $(this).addClass("active")
        $("#publicBtn").removeClass("active")
        $("#optionsBtn").removeClass("active")

        if (skipReviews == 0)
            loadReviews()
    })

    $("#showMoreReviewsCon").removeClass("d-none").fadeOut(0)

    $("#addToCart").on("click", function () {
        $.ajax({
            url: "/ShoppingCart/AddToCart",
            type: "post",
            data: $("#addToCartForm").serialize(),
            success: function (obj) {
                if (obj.isSucceeded) {
                    $("#main").prepend(`<div class="alert alert-success alert-dismissible">${obj.msg} <btn class="btn-close" data-bs-dismiss="alert"></btn></div>`).fadeOut(0).fadeIn("slow")

                    var cartItemsCount = $("#cartItemsCount")
                    cartItemsCount.text(new Number(cartItemsCount.text()) + 1).removeClass("d-none").fadeOut(0).fadeIn("slow")
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

    const take = 4
    var skipRelated = 0
    var skipOther = 0

    function addCards(container, items) {

        for (i = 0; i < items.length; ++i) {
            $(container).trigger("add.owl.carousel",`
                       <div class="item">
                            <a href="/shop/Details/${items[i].id}" class="text-decoration-none"><div class="card border-0">
                                <img src="/${items[i].imgUrl}" class="card-img-top w-100" style="height:200px;" />
                                <div class="card-body px-0">
                                    <h2 class="card-title text-capitalize text-primary mb-0 fs-5">${items[i].name}<span class="float-end ${items[i].discount != null ? "text-decoration-line-through text-muted small" : ""}">${items[i].price}$</span></h2>
                                    ${items[i].discount != null ? '<p class="card-text text-muted m-0">Discount: <span class="text-danger fs-6">' + items[i].discount + '%</span><span class="float-end text-primary fs-6">$' + (items[i].price - items[i].price * items[i].discount / 100) + '</span></p>' : ''}
                                </div>
                             </div></a>
                       </div>`).trigger("refresh.owl.carousel")
        }
    }

    function loadRelated() {
        $.ajax({
            type: "get",
            url: `/Shop/getItems?categoryId=${$("#categoryId").val()}&skip=${skipRelated}&take=${take}`,
            success: function (items) {

                addCards("#relatedProducts", items)

                skipRelated += take
            }
        })
    }

    function loadOther() {
        $.ajax({
            type: "get",
            url: `/shop/getItems?skip=${skipOther}&take=${take}`,
            success: function (items) {

                addCards("#otherProducts", items)

                skipOther += take
            }
        })
    }

    loadRelated()
    loadOther()

    $(".owl-carousel").owlCarousel({
        loop: false,
        margin: 15,
        nav: false,
        autoplay:true,
        responsive: {
            0: {
                items: 1
            },
            500: {
                items: 2
            },
            700: {
                items:3
            },
            1000: {
                items: 4
            }
        }
    })

    $("#relatedProducts").on("changed.owl.carousel", function (e) {
        if(e.item.count == e.item.index + e.page.size)
            loadRelated()
    })

    $("#otherProducts").on("changed.owl.carousel", function (e) {
        if (e.item.count == e.item.index + e.page.size)
            loadOther()
    })


})
