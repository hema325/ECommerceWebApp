
$(function () {

    const take = 9

    var skipDataContainer = 0
    var skipSearchContainer = 0

    function addCards(container,items) {
        for (i = 0; i < items.length; ++i) {
            $(container).append(`
                       <div class="col">
                            <a href="/shop/Details/${items[i].id}" class="text-decoration-none"><div class="card border-0 ">
                                <img src="/${items[i].imgUrl}" class="card-img-top" style="height:200px" />
                                <div class="card-body px-0">
                                    <h2 class="card-title text-capitalize text-primary mb-0 fs-5">${items[i].name}<span class="float-end ${items[i].discount != null ? "text-decoration-line-through text-muted small" : ""}">$${items[i].price}</span></h2>
                                    ${items[i].discount != null ? '<p class="card-text text-muted m-0">Discount: <span class="text-danger fs-6">' + items[i].discount + '%</span><span class="float-end text-primary fs-6">$' + (items[i].price - items[i].price*items[i].discount/100) +'</span></p>': ''}
                                </div>
                             </div></a>
                       </div>`)
        }
    }

    function controlShowMore(recievedCount) {
        if (recievedCount < take)
            $("#showMore").fadeOut("slow")
        else
            $("#showMore").removeClass("d-none").fadeOut(0).fadeIn("slow")
    }

    function loadData() {
        $.ajax({
            type: "get",
            url: `/Shop/getItems?categoryId=${$("#categoryId").val()}&sortBy=${$("#sortBy").val()}&minPrice=${$("#minPrice").val()}&maxPrice=${$("#maxPrice").val()}&skip=${skipDataContainer}&take=${take}`,
            success: function (items) {

                addCards("#cardsContainer", items)

                $("#cardsContainer").fadeOut(0).fadeIn("slow")

                controlShowMore(items.length)

                skipDataContainer += take

            }
        })
    }

    function searchData() {
        if ($("#searchInput").val().trim() != "") {
            $.ajax({
                type: "get",
                url: `/Shop/SearchItems?filter=${$("#searchInput").val().trim()}&skip=${skipSearchContainer}&take=${take}`,
                success: function (items) {

                    $("#cancel").removeClass("d-none").fadeOut(0).fadeIn("slow")
                    $("#cardsContainer").addClass("d-none")
                    $("#searchContainer").removeClass("d-none").fadeOut(0).fadeIn("slow")

                    addCards("#searchContainer", items)

                    controlShowMore(items.length)

                    skipSearchContainer += take
                }
            })
        }
    }

    loadData()

    $("#showMore").on("click", function () {
        if ($("#searchInput").val().trim() == "")
            loadData()
        else
            searchData()
    })

    $("#searchBtn").on("click", function () {
        $("#searchContainer").empty()
        skipSearchContainer = 0
        searchData()
    })

    $("#cancel").on("click", function () {
        $("#searchContainer").empty().fadeOut(0)
        $("#cardsContainer").removeClass("d-none").fadeOut(0).fadeIn("slow")
        $("#cancel").fadeOut("slow")
        $("#showMore").fadeIn("slow")
        $("#searchInput").val("")
    })

    $("#apply").on("click", function () {
        $("#searchContainer").empty().fadeOut(0)
        $("#cardsContainer").empty().removeClass("d-none")
        $("#cancel").fadeOut("slow")
        $("#showMore").fadeIn("slow")
        $("#searchInput").val("")
        skipDataContainer = 0
        loadData();
        $("#sortBy").val("-1")
        $("#minPrice").val("")
        $("#maxPrice").val("")
    })

})


