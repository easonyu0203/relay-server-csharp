namespace tcp_client;


public struct Payload
{
    public Int32 PayloadType;
    public Int32 BodySize;
    public byte[] Body;
    public Int32 EndFlag = END_FLAG;

    public Payload()
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

    public static byte[] Encode(ref Payload payload)
    {
        byte[] bOut = new byte[12 + payload.BodySize];
        byte[] bPayloadType = BitConverter.GetBytes(payload.PayloadType);
        byte[] bBodySize = BitConverter.GetBytes(payload.BodySize);
        byte[] bEndFlag = BitConverter.GetBytes(payload.EndFlag);
        Array.Copy(bPayloadType, 0, bOut, 0, 4);
        Array.Copy(bBodySize, 0, bOut, 4, 4);
        Array.Copy(payload.Body, 0, bOut, 8, payload.BodySize);
        Array.Copy(bEndFlag, 0, bOut, 8 + payload.BodySize, 4);
        
        return bOut;
    }

    public static Payload Decode(byte[] encodedPayload)
    {
        Payload pOut = new Payload();
        pOut.PayloadType = BitConverter.ToInt32(encodedPayload);
        pOut.BodySize = BitConverter.ToInt32(encodedPayload, 4);
        byte[] body = new byte[pOut.BodySize];
        Array.Copy(encodedPayload, 8, body, 0, pOut.BodySize);
        pOut.Body = body;
        return pOut;
    }
}