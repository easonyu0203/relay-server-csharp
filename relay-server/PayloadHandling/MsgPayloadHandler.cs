using relay_server.Payload;

namespace relay_server.PayloadHandling;

public class MsgPayloadHandler: IPayloadHandler
{
    public bool CanHandleType(BasePayload.Type type)
    {
        return type == BasePayload.Type.Msg;
    }

    public void HandlePayload(BasePayload recvBasePayload, RelayUser relayUser)
    {
        Console.WriteLine("[recv] msg => relaying");
        foreach (Room room in Hotel.Instance!.UserRooms[relayUser])
        {
            room.RelayPayload(recvBasePayload, relayUser);
        }
    }
}