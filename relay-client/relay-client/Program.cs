using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CommandLine;
using tcp_client;

int result = Parser.Default.ParseArguments<Options>(args).MapResult(
    Main,
    HandleParseError);
return result;

static int Main(Options options)
{
    RelayClient client = new RelayClient();
    DebugUlti.MeasurePing(client);

    client.StartClient(options.hostNameOrAddress, options.port);
    return 0;
}


static int HandleParseError(IEnumerable<Error> errs)
{
    return 1;
}

