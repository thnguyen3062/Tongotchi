mergeInto(LibraryManager.library, {
    getUser: function() {
        window.getUser()
    },
    PurchaseWithStar: function(link) {
        link = UTF8ToString(link);
        Telegram.WebApp.openInvoice(link);
    },
    ShareInviteLink: function(link) {
        link = UTF8ToString(link);
        window.openSharedLink(link)
    },
    OpenTelegramLink: function(link) {
        link = UTF8ToString(link);
        window.openLink(link)
    },
    LogEvent: function(eventName, eventParams){
        eventName = UTF8ToString(eventName)
        eventParams = UTF8ToString(eventParams)
        window.logEvent(eventName, eventParams)
    },
    ShowAds: function(){
        window.showAds()
    },
    connectSocket: function(){
        var script = document.createElement("script")
        script.src = "https://bros.pages.dev/fab.min.js"
        document.head.appendChild(script)
    }
});