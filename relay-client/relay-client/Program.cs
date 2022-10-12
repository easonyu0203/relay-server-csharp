using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using tcp_client;

RelayClient client = new RelayClient();
DebugUlti.MeasurePing(client);

client.StartClient();
