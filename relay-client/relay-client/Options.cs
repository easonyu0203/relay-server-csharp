namespace tcp_client;
using CommandLine;

public class Options
{
    [Option('h', "host", Required = false, Default = "localhost", HelpText = "specify the host name/address")]
    public string hostNameOrAddress { get; set; }
    [Option('p', "port", Required = false, Default = 3030, HelpText = "specify the port number ")]
    public int port { get; set; }
    
}