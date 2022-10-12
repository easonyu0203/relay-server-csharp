using CommandLine;
using relay_server;

int result = Parser.Default.ParseArguments<Options>(args).MapResult(
    Main,
    HandleParseError);
return result;

static int Main(Options options)
{
    Console.WriteLine("Success");
    Console.WriteLine($"host: {options.hostNameOrAddress}, port: {options.port}");
    RelayServer server = new RelayServer();
    server.StartServer(options.hostNameOrAddress, options.port);
    return 0;
}

static int HandleParseError(IEnumerable<Error> errs)
{
    return 1;
}