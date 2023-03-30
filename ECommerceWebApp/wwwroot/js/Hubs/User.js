
$(function () {
    var con = new signalR.HubConnectionBuilder().withUrl("/Hubs/User").build()

    con.on("setUserOnline", function (id) {
        $(".status" + id).removeClass("bg-success bg-danger").toggleClass("bg-success")
        $("#lastSeenContainer"+id).removeClass("d-none d-block").addClass("d-none").hide("slow")
    })

    con.on("setUserOffline", function (id) {
        $(".status" + id).removeClass("bg-success bg-danger").toggleClass("bg-danger")
    })

    con.on("RecieveMessage", function (msg) {
        if ($("#convId").val() == msg.conversationId) {

            $("#noMessages").addClass("d-none")

            //show message
            $("#chatBox")
                .append(`<div class="d-flex flex-row justify-content-start">
                            <div class="position-relative" style="width: 46px; height: 46px;">
                               <img src="/${msg.senderImgUrl}" class="rounded-circle d-block w-100 h-100"
                                alt="avatar 1">
                                <span class="rounded-circle border border-3 border-light position-absolute end-0 bottom-0 bg-success status${msg.senderId}" style="padding:6px"></span>
                            </div>
                            <div>
                                <p class="small p-2 ms-3 mb-1 rounded-3" style="background-color: #f5f6f7;">${msg.value}</p>
                                <p class="small ms-3 mb-3 rounded-3 text-muted float-end">${moment(msg.timeStamp).fromNow()}</p>
                            </div>
                        </div>`)

            //mark message as read
            $.ajax({
                type: "put",
                url: `/Conversation/MarkMessagAsRead?msgId=${msg.messageId}&convId=${msg.conversationId}`,
            })

            //scrolldown
            $("#chatBox").animate({ scrollTop: $("#chatBox").get(0).scrollHeight }, "slow")
        }
        else {
            $("#bill").removeClass("bg-danger bg-secondary").addClass("bg-danger");
            $("#convsLink").removeClass("text-danger text-muted").addClass("text-danger");
            $("#convHasUnReadMsgsCount").removeClass("d-none d-inline").addClass("d-inline").text("New");
        }
    })
    
    con.on("AddedToGroup", function (convId) {
        if ($("#convId").val() == convId) {
            $("#leftGroup").addClass("d-none")
            $("#sendMsgContainer").removeClass("d-none")
            $("#chatBox").animate({ scroll: $("#chatBox").get(0).scrollHeight }, "slow")
        }
    })

    con.on("LeftGroupMsg", function (convId, name) {
        if ($("#convId").val() == convId) {
            $("#chatBox").append(`<div class="text-center small text-muted">${name} has Left</div>`).animate({ scroll: $("#chatBox").get(0).scrollHeight }, "slow")
        }
    })

    con.on("SendMessage", function (msg) {

        if ($("#convId").val() == msg.conversationId) {

            $("#leaveGroupContainer").addClass("d-none")
            $("#noMessages").addClass("d-none");

            //show message
            $("#chatBox")
                .append(`<div class="d-flex flex-row justify-content-end">
                            <div>
                                 <p class="small p-2 me-3 mb-1 text-white rounded-3 bg-primary">${msg.value}</p>
                                 <p class="small me-3 mb-3 rounded-3 text-muted">
                                    ${moment(msg.timeStamp).fromNow()}
                                    <i id="marker${msg.messageId}" class="bi bi-check-all fs-5 text-secondary marker${msg.conversationId}"></i>
                                 </p>
                            </div>
                            <img src="${$("#senderImgUrl").attr("src")}" class="rounded-circle"
                             alt="avatar 1" style="width: 46px; height: 46px;">
                         </div>`)

            //empty formMessages
            $("#message").val("")

            //scrolldown
            $("#chatBox").animate({ scrollTop: $("#chatBox").get(0).scrollHeight }, "slow")
        }

    })

    con.on("MarkAsRead", function (msgId) {
        $("#marker" + msgId).removeClass("text-secondary text-success").addClass("text-success")
    })

    con.on("MarkAllAsRead", function (convId) {
        $(".marker" + (convId)).removeClass("text-secondary text-success").addClass("text-success")
    })

    con.on("ChangeConversationDetails", function (conv) {

        $("#conv" + conv.conversationId).remove()

        $("#convsContainer")
            .prepend(`<a href="/Conversation/Conversation?convId=${conv.conversationId}&receiverId=${conv.userId}" id="conv${conv.conversationId}" class="list-group-item list-group-item-action animateDown searchResult">
                            <div class="d-flex justify-content-between">
                                <div class="d-flex flex-row">
                                   <div class="position-relative" style="width:55px; height:55px;">
                                    <img
                                        src="/${conv.imgUrl}"
                                        alt="avatar" class="d-flex align-self-center rounded-circle d-block w-100 h-100">
                                      <span class="rounded-circle border border-light border-3 position-absolute bottom-0 end-0 p-2 bg-success status${conv.userId}"></span>
                                   </div>
                                   <div class="pt-1 ms-3">
                                      <p class="fw-bold mb-0 text-primary text-capitalize">${conv.name}</p>
                                      <p class="small text-muted" id="lastMessage${conv.conversationId}">${conv.lastMessage}</p>
                                   </div>
                                </div>
                                 <div class="pt-1">
                                    <p class="small text-muted mb-1 moment" id="timeStamp${conv.conversationId}">${moment(conv.messageTimeStamp).fromNow()}</p>
                                    <span class="badge rounded-pill float-end bg-info ${conv.unReadMessagesCount > 0 ? "d-inline" : "d-none"}" id="unReadMessagesCount${conv.conversationId}">${conv.unReadMessagesCount}</span>
                                </div>
                            </div>
                          </a>`).fadeOut(0).fadeIn("slow")
    })

    con.on("ChangeGroupDetails", function (group) {

        $("#conv" + group.conversationId).remove()

        $("#groupsContainer")
            .prepend(`<a href="/Conversation/Group?convId=${group.conversationId}&receiverId=${group.userId}" id="conv${group.conversationId}" class="list-group-item list-group-item-action animateDown searchResult">
                           <div class="d-flex justify-content-between">
                               <div class="d-flex flex-row">
                                  <div class="position-relative" style="width:55px; height:55px;">
                                   <img
                                       src="/${group.imgUrl}"
                                       alt="avatar" class="d-flex align-self-center rounded-circle d-block w-100 h-100">
                                  </div>
                                  <div class="pt-1 ms-3">
                                     <p class="fw-bold mb-0 text-primary text-capitalize">${group.name}</p>
                                     <p class="small text-muted" id="lastMessage${group.conversationId}">${group.lastMessage}</p>
                                  </div>
                               </div>
                                <div class="pt-1">
                                   <p class="small text-muted mb-1 moment" id="timeStamp${group.conversationId}">${moment(group.lastMessageTimeStamp).fromNow()}</p>
                                   <span class="badge rounded-pill float-end bg-info">New</span>
                               </div>
                           </div>
                      </a>`).fadeOut(0).fadeIn("slow")
    })

    con.start()

    $("#sendPrivate").on("click", function () {
        if ($("#message").val() != "") {

            //notify subscribers
            con.invoke("SendPrivate", $("#convId").val(), $("#receiverId").val(), $("#message").val())
        }
    })

    $("#sendGroup").on("click", function () {
        if ($("#message").val() != "") {

            //notify subscribers
            con.invoke("SendGroup", $("#convId").val(), $("#message").val())
        }
    })
})








