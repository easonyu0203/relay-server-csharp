using System.Net.Sockets;

namespace relay_server.PayloadHandling;

public class PingPayloadHandler : IPayloadHandler
{
    public bool CanHandleType(Payload.Type type)
    {
        return type == Payload.Type.Ping;
    }

    public void HandlePayload(Payload recvPayload, User user)
    {
        Console.WriteLine("[recv] Ping");
        // create ping payload
        Payload sendPayload = new Payload();
        sendPayload.PayloadType = (Int32)Payload.Type.Ping; 
        sendPayload.BodySize = 0;
        sendPayload.Body = new byte[] { };
        // send ping payload to client
        user.SendPayload(sendPayload);
        Console.WriteLine("[send] ping");
    }
}