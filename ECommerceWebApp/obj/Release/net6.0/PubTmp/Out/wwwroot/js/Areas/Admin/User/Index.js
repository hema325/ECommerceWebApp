


$(function () {

    const take = 8;
    var skipData = 0;
    var skipSearch = 0;

    var isDataContainer = true

    function addData(container,items)
    {
        for (i = 0; i < items.length; ++i) {
            $(container).append(`
                    <tr>
                        <td>
                            <img src="/${items[i].imgUrl}" width="36" height="36" class="rounded-circle"/>
                        </td>
                        <td class="text-capitalize">${items[i].name}</td>
                        <td>${items[i].email}</td>
                        <td>${items[i].phoneNumber}</td>
                        <td>
                            <a href="/Admin/User/Details/${items[i].id}" id="${items[i].Id}" class="btn btn-primary p-1 me-2" >Details</a>
                            <a href="/Admin/User/block/${items[i].id}" id="${items[i].Id}" class="btn btn-danger p-1 me-2 ${items[i].isBlocked ? "d-none" : ""}" >Block</a>
                            <a href="/Admin/User/Unblock/${items[i].id}" id="${items[i].Id}" class="btn btn-success p-1 ${items[i].isBlocked ? "" : "d-none"}" >UnBlock</a>
                        </td>
                    </tr>
            `)
        }

        $(container).fadeOut(0).fadeIn(1000)
    }

    function loadData() {
        $.ajax({
            type: "get",
            url: `/Admin/User/getUsers?skip=${skipData}&take=${take}`,
            success: function (items)
            {
                addData("#tbodyData",items)

                if(items.length< take)
                    $("#showMore").hide();

                skipData += take
            }
        })
    }

    loadData()
    $("#tbodySearch").hide(0)
    $("#cancelSearch").removeClass("d-none").hide(0)

    function searchData() {
        $.ajax({
            type: "get",
            url: `/Admin/User/SearchUsers?filter=${$("#searchInput").val()}&skip=${skipSearch}&take=${take}`,
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
        if ($("#searchInput").val().trim() != "")
        {
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

    $(".block").each(function () {
        $(this).click(function () {
            var userId = $(this).attr("id")
            alert(userId)
        })
    })
})




