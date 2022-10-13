namespace relay_server.Payload;

public class StatusPayload : BasePayload
{
    public StatusPayload(int statusCode)
    {
        PayloadType = (Int32)Type.Status;
        BodySize = 4;
        Body = BitConverter.GetBytes(statusCode);
    }
}