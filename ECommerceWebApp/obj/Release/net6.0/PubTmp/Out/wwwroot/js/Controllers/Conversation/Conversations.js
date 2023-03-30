

$(function () {

    const take = 8

    var skipConvContainer = 0
    var skipSearchContainer = 0
    var skipGroupContainer = 0

    var isGroupsLoaded = false

    var areYouInGroupsArea = false

    function addConvs(container, items) {
        for (i = 0; i < items.length; ++i) {
            $(container)
                .append(`<a href="Conversation?convId=${items[i].conversationId}&receiverId=${items[i].userId}" id="conv${items[i].conversationId}" class="list-group-item list-group-item-action animateDown searchResult">
                            <div class="d-flex justify-content-between">
                                <div class="d-flex flex-row">
                                   <div class="position-relative" style="width:55px; height:55px;">
                                    <img
                                        src="/${items[i].imgUrl}"
                                        alt="avatar" class="d-flex align-self-center rounded-circle d-block w-100 h-100">
                                      <span class="rounded-circle border border-light border-3 position-absolute bottom-0 end-0  ${items[i].isOnline ? "bg-success" : "bg-danger"} status${items[i].userId}" style="padding:8px;"></span>
                                   </div>
                                   <div class="pt-1 ms-3">
                                      <p class="fw-bold mb-0 text-primary text-capitalize">${items[i].userName}</p>
                                      <p class="small text-muted" id="lastMessage${items[i].conversationId}">${items[i].lastMessage != null ? items[i].lastMessage:"No Messages"}</p>
                                   </div>
                                </div>
                                 <div class="pt-1">
                                    <p class="small text-muted mb-1 moment ${items[i].messageTimeStamp == null ?"d-none":""}" id="timeStamp${items[i].conversationId}">${moment(items[i].messageTimeStamp).fromNow()}</p>
                                    <span class="badge rounded-pill float-end bg-info ${items[i].unReadMessagesCount > 0 ? "d-inline" : "d-none"}" id="unReadMessagesCount${items[i].conversationId}">${items[i].unReadMessagesCount}</span>
                                </div>
                            </div>
                          </a>`)
        }
    }

    function addGroups(container, items) {
        for (i = 0; i < items.length; ++i) {
            $(container)
                .append(`<a href="group?convId=${items[i].id}" id="conv${items[i].id}" class="list-group-item list-group-item-action animateDown searchResult">
                            <div class="d-flex justify-content-between">
                                <div class="d-flex flex-row">
                                   <div class="position-relative" style="width:55px; height:55px;">
                                    <img
                                        src="/${items[i].imgUrl}"
                                        alt="avatar" class="d-flex align-self-center rounded-circle d-block w-100 h-100">
                                      
                                   </div>
                                   <div class="pt-1 ms-3">
                                      <p class="fw-bold mb-0 text-primary text-capitalize">${items[i].name}</p>
                                      <p class="small text-muted" id="lastMessage${items[i].id}">${items[i].lastMessage != null ? items[i].lastMessage : "No Messages"}</p>
                                   </div>
                                </div>
                                 <div class="pt-1">
                                    <p class="small text-muted mb-1 moment ${items[i].messageTimeStamp == null ? "d-none" : ""}" id="timeStamp${items[i].id}">${moment(items[i].messageTimeStamp).fromNow()}</p>
                                    <span class="badge rounded-pill float-end bg-info ${items[i].unReadMessagesCount > 0 ? "d-inline" : "d-none"}" id="unReadMessagesCount${items[i].id}">${items[i].unReadMessagesCount}</span>
                                </div>
                            </div>
                          </a>`)
        }
    }

    function controlShowMore(recievedCount) {
        if (recievedCount < take)
            $("#showMore").addClass("d-none")
        else
            $("#showMore").removeClass("d-none").fadeOut(0).fadeIn("slow")
    }


    function loadConvs() {
        $.ajax({
            type: "get",
            url: `GetConversations?skip=${skipConvContainer}&take=${take}`,
            success: function (items) {
                addConvs("#convsContainer", items)

                controlShowMore(items.length)

                skipConvContainer += take

                $("#convsContainer").fadeOut(0).fadeIn("slow")
            }
        })
    }

    function searchConvs() {
        $.ajax({
            type: "get",
            url: `SearchConversations?filter=${$("#searchInput").val().trim()}&skip=${skipSearchContainer}&take=${take}`,
            success: function (items) {

                $("#searchContainer").removeClass("d-none").fadeOut(0).fadeIn("slow")
                addConvs("#searchContainer", items)

                controlShowMore(items.length)

                skipSearchContainer += take
            }
        })
    }

    function loadGroups() {
        $.ajax({
            type: "get",
            url: `GetGroups?skip=${skipGroupContainer}&take=${take}`,
            success: function (items) {
                addGroups("#groupsContainer", items)

                controlShowMore(items.length)

                skipGroupContainer += take

                $("#groupsContainer").fadeOut(0).fadeIn("slow")
            }
        })
    }

    function searchGroups() {
        $.ajax({
            type: "get",
            url: `SearchGroups?filter=${$("#searchInput").val()}&skip=${skipSearchContainer}&take=${take}`,
            success: function (items) {
                addGroups("#searchContainer", items)

                controlShowMore(items.length)

                skipSearchContainer += take

                $("#searchContainer").removeClass("d-none").fadeOut(0).fadeIn("slow")
            }
        })
    }

    loadConvs()

    //search users
    $("#searchBtn").on("click", function () {
        if ($("#searchInput").val().trim() != '')
        {
            $("#searchContainer").empty().removeClass("d-none")
            $("#convsContainer").addClass("d-none")
            $("#groupsContainer").addClass("d-none")
            skipSearchContainer = 0

            if (!areYouInGroupsArea) 
                searchConvs()
            

            if (areYouInGroupsArea)
                searchGroups()
            
        }
    })


    $("#showMore").on("click", function () {
        if ($("#searchInput").val().trim() == "") {
            if (!areYouInGroupsArea)
                loadConvs()
            else
                loadGroups()
        }
        else {
            if (!areYouInGroupsArea)
                searchConvs()
            else
                searchGroups()
        }

    })

    $("#showConvs").on("click", function () {
        $("#convsContainer").removeClass("d-none").fadeOut(0).fadeIn("slow")
        $("#groupsContainer").addClass("d-none")
        $("#searchContainer").addClass("d-none").empty()
        $("#searchInput").val("")
        $("#showMore").removeClass("d-none").fadeOut(0).fadeIn("slow")
        $("#createGroup").addClass("d-none")
        skipSearchContainer = 0

        areYouInGroupsArea = false
    })

    $("#showGroups").on("click", function () { 
        $("#convsContainer").addClass("d-none")
        $("#searchContainer").addClass("d-none").empty()
        $("#groupsContainer").removeClass("d-none")
        $("#searchInput").val("")
        $("#showMore").removeClass("d-none")
        $("#createGroup").removeClass("d-none")
        skipSearchContainer = 0

        if (!isGroupsLoaded) {
            loadGroups()
            $("#groupsContainer").empty()
            isGroupsLoaded = true
        }
        else {
            $("#groupsContainer").fadeOut(0).fadeIn("slow")
            $("#showMore").fadeOut(0).fadeIn("slow")
        }
        areYouInGroupsArea = true
    })

})