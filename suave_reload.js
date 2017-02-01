function init()
{
    websocket = new WebSocket("ws://"+window.location.host+"/websocket");
    websocket.onmessage = function(evt) { location.reload(); };
}
window.addEventListener("load", init, false);