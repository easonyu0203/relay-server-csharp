using System.Net;
using System.Net.Sockets;
using System.Text;
using relay_server.Payload;
using relay_server.PayloadHandling;

namespace relay_server;

public class RelayServer
{
    public RelayServer()
    {
        Hotel hotel = new Hotel();
    }

    public IPayloadHandler[] GetPayloadHandlers => new IPayloadHandler[]
    {
        new ClosePayloadHandler(),
        new PingPayloadHandler(),
        new JoinPayloadHandler(),
        new LeavePayloadHandler(),
        new MsgPayloadHandler(),
    };

    public void StartServer(string hostNameOrAddress, int port)
    {
        IPHostEntry host = Dns.GetHostEntry(hostNameOrAddress);
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
        // listen for connection
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEndPoint);
        listener.Listen(100);
        Console.WriteLine("Waiting for a connection...");

        // accept connection
        while (true)
        {
            Socket clientHandler = listener.Accept();
            // run client handler on another thread
            Task.Run(() => HandleAcceptConnection(clientHandler));
        }
    }


    public void HandleAcceptConnection(Socket clientHandler)
    {
        try
        {
            Console.WriteLine("accept client connection");
            // Create User
            RelayUser relayUser = new RelayUser(clientHandler);
            // set up handler
            IPayloadHandler[] payloadHandlers = GetPayloadHandlers;

            // client server communication
            while (relayUser.Connected)
            {
                BasePayload recvBasePayload = relayUser.ReceivePayload();
                // do corresponding actions
                foreach (IPayloadHandler payloadHandler in payloadHandlers)
                {
                    if (payloadHandler.CanHandleType((BasePayload.Type)recvBasePayload.PayloadType))
                    {
                        payloadHandler.HandlePayload(recvBasePayload, relayUser);
                        break;
                    }
                }
            }

            Console.WriteLine("client connection end");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}