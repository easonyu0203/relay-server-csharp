using System.Diagnostics;
using System.Net.Sockets;

namespace tcp_client;

public static class DebugUlti
{
    static public void MeasurePing(RelayClient client)
    {
        Stopwatch stopwatch = new Stopwatch();
        client.SendPingEvent += () =>
        {
            stopwatch.Restart();
        };
        client.RecvPingEvent += () =>
        {
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine($"ping: {ts.Milliseconds} ms");
        };

    }
}