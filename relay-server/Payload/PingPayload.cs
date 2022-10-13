namespace relay_server.Payload;

public class PingPayload : BasePayload
{
    public PingPayload()
    {
        PayloadType = (Int32)Type.Ping;
        BodySize = 0;
        Body = new byte[] { };
    }
}