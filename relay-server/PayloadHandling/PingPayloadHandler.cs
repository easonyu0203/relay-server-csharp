using System.Net.Sockets;
using relay_server.Payload;

namespace relay_server.PayloadHandling;

public class PingPayloadHandler : IPayloadHandler
{
    public bool CanHandleType(BasePayload.Type type)
    {
        return type == BasePayload.Type.Ping;
    }

    public void HandlePayload(BasePayload recvBasePayload, RelayUser relayUser)
    {
        Console.WriteLine("[recv] Ping");
        // send ping payload to client
        relayUser.SendPayload(new PingPayload());
        Console.WriteLine("[send] ping");
    }
}