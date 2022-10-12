using System.Net.Sockets;

namespace relay_server.PayloadHandling;

public interface IPayloadHandler
{
    public bool CanHandleType(Payload.Type type);
    public void HandlePayload(Payload recvPayload, User user);
}