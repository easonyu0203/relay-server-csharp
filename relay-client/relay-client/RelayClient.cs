using System.Net;
using System.Net.Sockets;
using System.Text;

namespace relay_client;

public class RelayClient
{
    public event Action SendPingEvent;
    public event Action RecvPingEvent;

    public void StartClient(string hostName, int port)
    {
        byte[] buffer = new byte[1048576]; // 2MB

        try
        {
            IPHostEntry host = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            Socket socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}",
                    socket.RemoteEndPoint);
                var sendThr = Task.Run(() =>
                {
                    while (socket.Connected)
                    {
                        BasePayload sendPayload = new BasePayload();
                        // do user command
                        Console.Write("\n>> ");
                        string? line = Console.ReadLine();
                        if(line == "") continue;
                        if (line == null) continue;
                        string[] worlds = line.Split(" ");
                        switch (worlds[0].ToUpper())
                        {
                            case "PING":
                                Console.WriteLine("[send] Ping request");
                                // delegate
                                SendPingEvent?.Invoke();
                                // send 
                                socket.Send(BasePayload.Encode(new PingPayload()));
                                break;
                            case "CLOSE":
                                Console.WriteLine("[send] close request");
                                // send 
                                socket.Send(BasePayload.Encode(new ClosePayload()));
                                break;
                            case "JOIN":
                                Console.WriteLine("[send] join request");
                                int roomId = int.Parse(worlds[1]);
                                socket.Send(BasePayload.Encode(new JoinPayload(roomId)));
                                break;
                            case "LEAVE":
                                Console.WriteLine("[send] leave request");
                                roomId = int.Parse(worlds[1]);
                                socket.Send(BasePayload.Encode(new LeavePayload(roomId)));
                                break;
                            case "MSG":
                                Console.WriteLine("[send] msg request");
                                byte[] body = Encoding.ASCII.GetBytes(line.Substring(line.IndexOf(' ') + 1));
                                socket.Send(BasePayload.Encode(new MsgPayload(body)));
                                break;
                            default:
                                Console.WriteLine("unknown command");
                                continue;
                        }
                    }
                });
                var recvThr = Task.Run(() =>
                {
                    while (socket.Connected)
                    {
                        BasePayload recvPayload = new BasePayload();
                        // receive
                        recvPayload = ReceivePayload(socket, ref buffer);
                        Console.WriteLine("\n=========");
                        // handle received payload
                        switch ((BasePayload.Type)recvPayload.PayloadType)
                        {
                            case BasePayload.Type.Ping:
                                Console.WriteLine("[recv] ping");
                                RecvPingEvent?.Invoke();
                                break;
                            case BasePayload.Type.Close:
                                Console.WriteLine("[recv] close");
                                socket.Shutdown(SocketShutdown.Both);
                                socket.Close();
                                break;
                            case BasePayload.Type.Status:
                                Console.WriteLine("[recv] status");
                                Console.WriteLine($"get status code: {BitConverter.ToInt32(recvPayload.Body)}");
                                break;
                            case BasePayload.Type.Msg:
                                Console.WriteLine("[recv] msg");
                                Console.WriteLine(Encoding.ASCII.GetString(recvPayload.Body));
                                break;
                            default:
                                Console.WriteLine($"not support payload type: {(BasePayload.Type)recvPayload.PayloadType}");
                                break;
                        }
                        Console.WriteLine("\n=========");
                    }
                });
                sendThr.Wait();
                recvThr.Wait();

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    BasePayload ReceivePayload(Socket socket, ref byte[] lBuffer)
    {
        int byteSize = 0;
        while (true)
        {
            int bytesRec = socket.Receive(lBuffer, byteSize, lBuffer.Length - byteSize, SocketFlags.None);
            byteSize += bytesRec;
            if (byteSize >= 4)
            {
                int lastInt = BitConverter.ToInt32(lBuffer, byteSize - 4);
                if (lastInt == BasePayload.END_FLAG)
                    break;
            }
        }

        return BasePayload.Decode(lBuffer);
    }
}