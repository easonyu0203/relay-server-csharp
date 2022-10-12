namespace relay_server.PayloadHandling;

public class MsgPayloadHandler: IPayloadHandler
{
    public bool CanHandleType(Payload.Type type)
    {
        return type == Payload.Type.Msg;
    }

    public void HandlePayload(Payload recvPayload, User user)
    {
        Console.WriteLine("[recv] msg => relaying");
        foreach (Room room in Hotel.Instance.UserRooms[user])
        {
            room.RelayPayload(recvPayload, user);
        }
    }
}