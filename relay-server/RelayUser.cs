using System.Net.Sockets;
using relay_server.Payload;

namespace relay_server;

public class RelayUser
{
    private static int _idCounter = 0;
    private static int GetNewId => _idCounter++;

    private int Id { get; }
    private Socket Socket { get; }
    private byte[] _buffer;

    public event Action OnDisconnect;
    public bool Connected => Socket.Connected;

    public RelayUser(Socket socket)
    {
        Id = GetNewId;
        Socket = socket;
        _buffer = new byte[1048576]; // 2MB buffer
    }

    public BasePayload ReceivePayload()
    {
        try
        {
            int byteSize = 0;
            while (true)
            {
                int bytesRec = Socket.Receive(_buffer, byteSize, _buffer.Length - byteSize, SocketFlags.None);
                byteSize += bytesRec;
                if (byteSize >= 4)
                {
                    int lastInt = BitConverter.ToInt32(_buffer, byteSize - 4);
                    if (lastInt == BasePayload.END_FLAG)
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        return BasePayload.Decode(_buffer);
    }

    public void SendPayload(BasePayload basePayload)
    {
        Socket.Send(BasePayload.Encode(basePayload));
    }

    public void Disconnect()
    {
        OnDisconnect?.Invoke();
        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
    }
}