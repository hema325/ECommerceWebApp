

$(function () {

    const take = 20
    var skip = 0

    function addNoMsgsAvailable() {
        $("#chatBox").prepend(`
                <div class="w-100 d-flex align-items-center justify-content-center" style="height:95%;" id="noMessages">
                    <p class="text-muted">No Messages Are Available</p> 
                </div>`).fadeOut(0).fadeIn("slow")
    }

    function addReceiverMsg(msg) {
        $("#chatBox").prepend (`
            <div class="d-flex flex-row justify-content-start" id="${msg.id}">
                <div class="position-relative" style="width: 46px; height: 46px;">
                   <img src="/${msg.senderImgUrl}" class="rounded-circle d-block w-100 h-100"
                    alt="avatar 1">
                    <span class="rounded-circle border border-3 border-light position-absolute end-0 bottom-0 ${msg.senderIsOnline?"bg-success":"bg-danger"} status${msg.senderId}" style="padding:6px"></span>
                </div>
                <div>
                    <p class="small p-2 ms-3 mb-1 rounded-3" style="background-color: #f5f6f7;">${msg.value}</p>
                    <p class="small ms-3 mb-3 rounded-3 text-muted float-end"><span>${moment(msg.timeStamp).fromNow()}</span></p>
                </div>
            </div>
        `)
    }

    function addSenderMsg(msg) {
        $("#chatBox").prepend(`
            <div class="d-flex flex-row justify-content-end" id="${msg.id}">
                <div>
                    <p class="small p-2 me-3 mb-0 text-white rounded-3 bg-primary">${msg.value}</p>
                    <p class="small me-3 mb-3 rounded-3 text-muted d-flex align-items-center">
                        <span>${ moment(msg.timeStamp).fromNow() }</span >
                        <i id="marker${msg.id}" class="bi bi-check-all fs-5 ${msg.isRead ? " text-success" : "text-secondary"} marker${$("#convId").val()}"></i >
                    </p>
                </div >
            <img src="/${msg.senderImgUrl}" class="rounded-circle"
                alt="avatar 1" style="width: 46px; height: 46px;">
            </div>
        `)
    }

    function loadMsgs(wantScroll) {
        $.ajax({
            type: "get",
            url: `/Conversation/getConversationMsgs?convId=${$("#convId").val()}&skip=${skip}&take=${take}`,
            success: function (msgs) {
                for (i = 0; i < msgs.length; ++i) {
                    if (msgs[i].senderId == $("#senderId").val())
                        addSenderMsg(msgs[i])
                    else 
                        addReceiverMsg(msgs[i])
                }

                $("#showMoreContainer").prependTo("#chatBox")

                if (skip == 0 && msgs.length == 0)
                    addNoMsgsAvailable()

                if (msgs.length == take) 
                    $("#showMoreContainer").removeClass("d-none")

                if (msgs.length < take)
                    $("#showMoreContainer").addClass("d-none").fadeOut("slow")

                if (wantScroll)
                    $("#chatBox").animate({ scrollTop: $("#chatBox").get(0).scrollHeight },0)

                newMsgsCount = msgs.length
                skip += take
            }
        })
    }

    loadMsgs(true)

    $(".lastSeen").removeClass("invisible")

    $("#showMore").on("click", function () {
        loadMsgs()
        $("#chatBox").animate({ scrollTop: $("#chatBox").get(0).scrollHeight }, 0).fadeOut(0).fadeIn("slow")
    })

})






    




