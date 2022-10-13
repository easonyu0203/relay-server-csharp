namespace relay_server.Payload;

public class MsgPayload : BasePayload
{
    public MsgPayload(byte[] body)
    {
        PayloadType = (Int32)Type.Msg;
        BodySize = body.Length;
        Body = body;
    }
}