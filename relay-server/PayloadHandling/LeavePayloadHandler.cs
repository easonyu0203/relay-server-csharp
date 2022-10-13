using relay_server.Payload;

namespace relay_server.PayloadHandling;

public class LeavePayloadHandler : IPayloadHandler
{
    public bool CanHandleType(BasePayload.Type type)
    {
        return type == BasePayload.Type.Leave;
    }

    public void HandlePayload(BasePayload recvBasePayload, RelayUser relayUser)
    {
        Console.WriteLine("[recv] Leave request");
        int roomId = BitConverter.ToInt32(recvBasePayload.Body);
        if (Hotel.Instance == null)
        {
            Console.WriteLine("Hotel singleton not found");
        }

        Console.WriteLine($"leave room: {roomId}");
        bool success = Hotel.Instance != null && Hotel.Instance.LeaveRoom(roomId, relayUser);
        // send success
        BasePayload sendBasePayload = new StatusPayload(success ? 200 : 404);
        relayUser.SendPayload(sendBasePayload);
        
        Console.WriteLine(success?"[send] status success": "[send] status fail");
    }
}