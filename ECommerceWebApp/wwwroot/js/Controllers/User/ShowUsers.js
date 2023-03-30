
$(function () {

    const take = 9

    var skipDataContainer = 0
    var skipSearchContainer = 0

    function addCards(container,items) {
        for (i = 0; i < items.length; ++i) {
            $(container).append(`
                       <div class="col">
                            <div class="card border-0">
                                <img src="/${items[i].imgUrl}" class="card-img-top" style="height:250px" />
                                <div class="card-body px-0">
                                    <h2 class="card-title text-capitalize mb-0 fs-5">${items[i].name}</h2>
                                    <div class="d-flex mt-3 justify-content-end">
                                        <a href="/conversation/addConversation?userId2=${items[i].id}" class="btn btn-primary">Message</a>
                                    </div>
                                </div>
                             </div>
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
            url: `/User/GetUsers?skip=${skipDataContainer}&take=${take}`,
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
                url: `/User/SearchUsers?filter=${$("#searchInput").val().trim()}&skip=${skipSearchContainer}&take=${take}`,
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
})


