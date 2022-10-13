using relay_server.Payload;

namespace relay_server.PayloadHandling;

public class JoinPayloadHandler : IPayloadHandler
{
    public bool CanHandleType(BasePayload.Type type)
    {
        return type == BasePayload.Type.Join;
    }

    public void HandlePayload(BasePayload recvBasePayload, RelayUser relayUser)
    {
        Console.WriteLine("[recv] Join request");
        int roomId = BitConverter.ToInt32(recvBasePayload.Body);
        if (Hotel.Instance == null)
        {
            Console.WriteLine("Hotel singleton not found");
        }

        Console.WriteLine($"add to room: {roomId}");
        Hotel.Instance?.JoinRoom(roomId, relayUser);
        // send success
        relayUser.SendPayload(new StatusPayload(200));
        Console.WriteLine("[send] status success");
    }
}