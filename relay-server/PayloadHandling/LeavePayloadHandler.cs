namespace relay_server.PayloadHandling;

public class LeavePayloadHandler : IPayloadHandler
{
    public bool CanHandleType(Payload.Type type)
    {
        return type == Payload.Type.Leave;
    }

    public void HandlePayload(Payload recvPayload, User user)
    {
        Console.WriteLine("[recv] Leave request");
        int roomId = BitConverter.ToInt32(recvPayload.Body);
        if (Hotel.Instance == null)
        {
            Console.WriteLine("Hotel singleton not found");
        }

        Console.WriteLine($"leave room: {roomId}");
        bool success = Hotel.Instance != null && Hotel.Instance.LeaveRoom(roomId, user);
        // send success
        Payload sendPayload = new Payload()
        {
            PayloadType = (Int32)Payload.Type.Status,
            BodySize = 4,
            Body = BitConverter.GetBytes(success?200:404)
        };
        user.SendPayload(sendPayload);
        
        Console.WriteLine(success?"[send] status success": "[send] status fail");
    }
}