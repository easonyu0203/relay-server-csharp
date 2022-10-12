using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp_client;

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
                        Payload sendPayload = new Payload();
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
                                sendPayload.PayloadType = (Int32)Payload.Type.Ping;
                                sendPayload.BodySize = 0;
                                sendPayload.Body = new byte[] { };
                                // delegate
                                SendPingEvent?.Invoke();
                                // send 
                                socket.Send(Payload.Encode(ref sendPayload));
                                break;
                            case "CLOSE":
                                Console.WriteLine("[send] close request");
                                sendPayload.PayloadType = (Int32)Payload.Type.Close;
                                sendPayload.BodySize = 0;
                                sendPayload.Body = new byte[] { };
                                // send 
                                socket.Send(Payload.Encode(ref sendPayload));
                                break;
                            case "JOIN":
                                Console.WriteLine("[send] join request");
                                int roomId = int.Parse(worlds[1]);
                                sendPayload.PayloadType = (Int32)Payload.Type.Join;
                                sendPayload.BodySize = 4;
                                sendPayload.Body = BitConverter.GetBytes(roomId);
                                socket.Send(Payload.Encode(ref sendPayload));
                                break;
                            case "LEAVE":
                                Console.WriteLine("[send] leave request");
                                roomId = int.Parse(worlds[1]);
                                sendPayload.PayloadType = (Int32)Payload.Type.Leave;
                                sendPayload.BodySize = 4;
                                sendPayload.Body = BitConverter.GetBytes(roomId);
                                socket.Send(Payload.Encode(ref sendPayload));
                                break;
                            case "MSG":
                                Console.WriteLine("[send] msg request");
                                sendPayload.PayloadType = (Int32)Payload.Type.Msg;
                                byte[] body = Encoding.ASCII.GetBytes(line.Substring(line.IndexOf(' ') + 1));
                                sendPayload.BodySize = body.Length;
                                sendPayload.Body = body;
                                socket.Send(Payload.Encode(ref sendPayload));
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
                        Payload recvPayload = new Payload();
                        // receive
                        recvPayload = ReceivePayload(socket, ref buffer);
                        Console.WriteLine("\n=========");
                        // handle received payload
                        switch ((Payload.Type)recvPayload.PayloadType)
                        {
                            case Payload.Type.Ping:
                                Console.WriteLine("[recv] ping");
                                RecvPingEvent?.Invoke();
                                break;
                            case Payload.Type.Close:
                                Console.WriteLine("[recv] close");
                                socket.Shutdown(SocketShutdown.Both);
                                socket.Close();
                                break;
                            case Payload.Type.Status:
                                Console.WriteLine("[recv] status");
                                Console.WriteLine($"get status code: {BitConverter.ToInt32(recvPayload.Body)}");
                                break;
                            case Payload.Type.Msg:
                                Console.WriteLine("[recv] msg");
                                Console.WriteLine(Encoding.ASCII.GetString(recvPayload.Body));
                                break;
                            default:
                                Console.WriteLine($"not support payload type: {(Payload.Type)recvPayload.PayloadType}");
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

    Payload ReceivePayload(Socket socket, ref byte[] lBuffer)
    {
        int byteSize = 0;
        while (true)
        {
            int bytesRec = socket.Receive(lBuffer, byteSize, lBuffer.Length - byteSize, SocketFlags.None);
            byteSize += bytesRec;
            if (byteSize >= 4)
            {
                int lastInt = BitConverter.ToInt32(lBuffer, byteSize - 4);
                if (lastInt == Payload.END_FLAG)
                    break;
            }
        }

        return Payload.Decode(lBuffer);
    }
}