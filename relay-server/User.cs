using System.Net.Sockets;

namespace relay_server;

public class User
{
    private static int _idCounter = 0;
    private static int GetNewId => _idCounter++;

    private int Id { get; }
    private Socket Socket { get; }
    private byte[] _buffer;

    public event Action OnDisconnect;
    public bool Connected => Socket.Connected;

    public User(Socket socket)
    {
        Id = GetNewId;
        Socket = socket;
        _buffer = new byte[1048576]; // 2MB buffer
    }

    public Payload ReceivePayload()
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
                    if (lastInt == Payload.END_FLAG)
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        return Payload.Decode(_buffer);
    }

    public void SendPayload(Payload payload)
    {
        Socket.Send(Payload.Encode(ref payload));
    }

    public void Disconnect()
    {
        OnDisconnect?.Invoke();
        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
    }
}