"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/Chat").build();  
  
//Disable send button until connection is established  
document.getElementById("sendBtn").disabled = true;  



function updateScroll() {
    var element = document.getElementById("rows");
    element.scrollTop = element.scrollHeight;
}

  //postid, usename, date, message
connection.on("ReceiveMessage", function (postid, username, date, message, userid) {  

    /*
        <div class="row border border-primary">
            <div class="col-3 border-right border-primary">
                <b>User:</b> <text>@post.OwnerID</text> <br />
                <b>Date Posted:</b> <text> @post.Date </text><br />
            </div>
            <div class="col-9">
                <text>@post.Postmessage</text>
            </div>
        </div>
    */
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var checkuserid = $('input[name="UserID"]', form).val();
    var variable = "";
    if (Mod || Owner || Admin || userid == checkuserid) {
        variable = '<!DOCTYPE html><html><head></head><body>' +
            '<div class="row border border-primary">' +
            '    <div class="col-3 border-right border-primary" style="font-size:80%">' +
            '       <b>User:</b> <text>' + username + '</text> <br />' +
            '       <b>Date Posted:</b> <text>' + date + '</text><br />' +
            '       <form class="form-inline" method="post" action="/GroupView/Chat">' +
            '           <input type="hidden" name="ID" value="' + GroupID + '" />' +
            '           <input type="hidden" name="todelete" value="' + postid + '" />' +
            '           <div class="ml-auto">' +
            '               <button class="btn btn-info btn-sm mb-1" type="submit">' +
            '               <i class="fas fa-trash-alt"></i>' +
            '               </button>' +
            '           </div>' +
            '           <input name="__RequestVerificationToken" type="hidden" value="' + token + '"/>' +
            '       </form>' +
            '    </div>' +
            '    <div class="col-9">' +
            '        <pre>' + message + '</pre>' +
            '    </div>' +
            '</div>' +
            '</body></html>';
        
    }
    else {
        variable = '<!DOCTYPE html><html><head></head><body>' +
            '<div class="row border border-primary">' +
            '    <div class="col-3 border-right border-primary" style="font-size:80%">' +
            '        <b>User:</b> <text>' + username + '</text> <br />' +
            '        <b>Date Posted:</b> <text>' + date + '</text><br />' +
            '    </div>' +
            '    <div class="col-9">' +
            '        <pre>' + message + '</pre>' +
            '    </div>' +
            '</div>' +
            '</body></html>';
    }
    var newrow = new DOMParser().parseFromString(variable, 'text/html');
    document.getElementById("rows").innerHTML += (newrow.body.innerHTML);
    updateScroll();
});  
  
connection.start().then(function () {  
    document.getElementById("sendBtn").disabled = false;
    connection.invoke("Join", GroupID);
}).catch(function (err) {  
    return console.error(err.toString());  
});  
  
document.getElementById("sendBtn").addEventListener("click", function (event) {      
    var message = document.getElementById("txtmessage").value;
    connection.invoke("SendMessage", message, GroupID).catch(function (err) {  
        return console.error(err.toString());  
    });  
    event.preventDefault();  

    document.getElementById("txtmessage").value = "";
}); 

window.onbeforeunload = function (event) {
    connection.invoke("Leave", GroupID)
}

window.onload = updateScroll();

document.getElementById("txtmessage").addEventListener("keydown", (function (e) {
    // Enter was pressed without shift key
    if (e.key == 'Enter' && !e.shiftKey) {
        // prevent default behavior
        e.preventDefault();
        document.getElementById("sendBtn").click();
    }
}));


//<input name="__RequestVerificationToken" type="hidden" value="