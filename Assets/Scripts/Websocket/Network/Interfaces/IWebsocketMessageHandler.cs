using Game.Websocket.Model;
using UnityEngine;

namespace Game.Websocket.Interface
{
    public interface IWebSocketMessageHandler
    {
        string Group {  get; }
        void ParseMessage(SocketResponse response);
    }
}