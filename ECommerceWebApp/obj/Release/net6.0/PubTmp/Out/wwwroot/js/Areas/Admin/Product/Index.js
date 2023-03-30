

$(function () {

    const take = 8
    var skipData = 0
    var skipSearch = 0 

    var isDataContainer = true

    function addData(container, items) {
        for (i = 0; i < items.length; ++i) {
            $(container).append(`
            <tr>
                <td>
                    ${items[i].name}
                </td>
                <td>
                    ${items[i].category}
                </td>
                <td>
                    <a href="/Admin/Product/Details/${items[i].id}" class="text-primary me-2" ><i class="bi bi-info-square fs-5"></i></a>
                    <a href="/Admin/Product/edit/${items[i].id}" class="text-secondary me-2" ><i class="bi bi-pencil-square fs-5"></i></a>
                    <a href="/Admin/Product/delete/${items[i].id}" class="text-danger" ><i class="bi bi-trash fs-5"></i></a>
                </td>
            </tr>
            `)

        }

            $(container).fadeOut(0).fadeIn(1000)

    }

    function loadData() {
        $.ajax({
            type: "get",
            url: `/Admin/Product/GetProducts?skip=${skipData}&take=${take}`,
            success: function (items) {
                addData("#tbodyData", items)

                if (items.length < take)
                    $("#showMore").hide();

                skipData += items.length
            }
        })
    }

    loadData()
    $("#tbodySearch").hide(0)
    $("#cancelSearch").removeClass("d-none").hide(0)

    function searchData() {
        $.ajax({
            type: "get",
            url: `/Admin/Product/SearchProducts?filter=${$("#searchInput").val()}&skip=${skipSearch}&take=${take}`,
            success: function (items) {

                addData("#tbodySearch", items)


                if (items.length < take)
                    $("#showMore").hide();

                skipSearch += take
            }
        })
    }

    $("#showMore").on("click", function () {
        if (isDataContainer)
            loadData()
        else
            searchData()
    })

    $("#searchBtn").on("click", function () {
        if ($("#searchInput").val().trim() != "") {
            $("#tbodySearch").empty().show("fast")
            $("#tbodyData").hide(0)
            $("#cancelSearch").show("fast")
            $("#showMore").show("fast")

            isDataContainer = false
            skipSearch = 0
            searchData()
        }
    })

    $("#cancelSearch").on("click", function () {
        $("#tbodySearch").empty().hide(0)
        $("#tbodyData").show("fast")
        $("#cancelSearch").hide("fast")
        $("#showMore").show("fast")
        $("#searchInput").val("")

        isDataContainer = true
    })

})










