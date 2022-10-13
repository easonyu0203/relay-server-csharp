using System.Net.Sockets;
using relay_server.Payload;

namespace relay_server.PayloadHandling;

public interface IPayloadHandler
{
    public bool CanHandleType(BasePayload.Type type);
    public void HandlePayload(BasePayload recvBasePayload, RelayUser relayUser);
}