namespace relay_server.PayloadHandling;

public class JoinPayloadHandler : IPayloadHandler
{
    public bool CanHandleType(Payload.Type type)
    {
        return type == Payload.Type.Join;
    }

    public void HandlePayload(Payload recvPayload, User user)
    {
        Console.WriteLine("[recv] Join request");
        int roomId = BitConverter.ToInt32(recvPayload.Body);
        if (Hotel.Instance == null)
        {
            Console.WriteLine("Hotel singleton not found");
        }

        Console.WriteLine($"add to room: {roomId}");
        Hotel.Instance?.JoinRoom(roomId, user);
        // send success
        Payload sendPayload = new Payload()
        {
            PayloadType = (Int32)Payload.Type.Status,
            BodySize = 4,
            Body = BitConverter.GetBytes(200)
        };
        user.SendPayload(sendPayload);
        Console.WriteLine("[send] status success");
    }
}