namespace relay_server.Payload;

public class JoinPayload : BasePayload
{
    public JoinPayload(int roomId)
    {
        PayloadType = (Int32)Type.Join;
        BodySize = 4;
        Body = BitConverter.GetBytes(roomId);
    }
}