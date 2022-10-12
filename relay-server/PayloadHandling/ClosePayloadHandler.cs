using System.Net.Sockets;

namespace relay_server.PayloadHandling;

public class ClosePayloadHandler : IPayloadHandler
{
    public bool CanHandleType(Payload.Type type)
    {
        return type == Payload.Type.Close;
    }

    public void HandlePayload(Payload recvPayload, User user)
    {
        Console.WriteLine("[recv] request close");
        // create close payload
        Payload sendPayload = new Payload();
        sendPayload.PayloadType = (Int32)Payload.Type.Close;
        sendPayload.BodySize = 0;
        sendPayload.Body = new byte[] { };
        // send close payload to client
        user.SendPayload(sendPayload);
        // close client handler
        user.Disconnect();
        Console.WriteLine("[send] close");
    }
}