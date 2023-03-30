




$(function () {
    $.ajax({
        type: "get",
        url: "/User/getRelatedUsers",
        success: function (items) {
            for (i = 0; i < items.length; ++i) {
                $("#UsersIDs").append (`
                    <option value="${items[i].id}">${items[i].name}</option>
                `)
            }
        }
    })
})
