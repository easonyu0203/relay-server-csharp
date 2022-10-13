namespace relay_server.Payload;

public class BasePayload
{
    public Int32 PayloadType;
    public Int32 BodySize;
    public byte[] Body;
    public Int32 EndFlag = END_FLAG;

    public BasePayload()
    {
        PayloadType = 0;
        BodySize = 0;
        Body = new byte[] { };
    }

    public enum Type : Int32
    {
        Msg,
        Join,
        Leave,
        Ping,
        Close,
        Status,
    }

    public static Int32 END_FLAG = -11;

    public static byte[] Encode(BasePayload basePayload)
    {
        byte[] bOut = new byte[12 + basePayload.BodySize];
        byte[] bPayloadType = BitConverter.GetBytes(basePayload.PayloadType);
        byte[] bBodySize = BitConverter.GetBytes(basePayload.BodySize);
        byte[] bEndFlag = BitConverter.GetBytes(basePayload.EndFlag);
        Array.Copy(bPayloadType, 0, bOut, 0, 4);
        Array.Copy(bBodySize, 0, bOut, 4, 4);
        Array.Copy(basePayload.Body, 0, bOut, 8, basePayload.BodySize);
        Array.Copy(bEndFlag, 0, bOut, 8 + basePayload.BodySize, 4);

        return bOut;
    }

    public static BasePayload Decode(byte[] encodedPayload)
    {
        BasePayload pOut = new BasePayload();
        pOut.PayloadType = BitConverter.ToInt32(encodedPayload);
        pOut.BodySize = BitConverter.ToInt32(encodedPayload, 4);
        byte[] body = new byte[pOut.BodySize];
        Array.Copy(encodedPayload, 8, body, 0, pOut.BodySize);
        pOut.Body = body;
        return pOut;
    }
}