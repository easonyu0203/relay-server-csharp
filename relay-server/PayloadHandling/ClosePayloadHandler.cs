using System.Net.Sockets;
using relay_server.Payload;

namespace relay_server.PayloadHandling;

public class ClosePayloadHandler : IPayloadHandler
{
    public bool CanHandleType(BasePayload.Type type)
    {
        return type == BasePayload.Type.Close;
    }

    public void HandlePayload(BasePayload recvBasePayload, RelayUser relayUser)
    {
        Console.WriteLine("[recv] request close");
        // send close payload to client
        relayUser.SendPayload(new ClosePayload());
        // close client handler
        relayUser.Disconnect();
        Console.WriteLine("[send] close");
    }
}