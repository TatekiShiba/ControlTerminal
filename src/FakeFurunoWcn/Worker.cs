using System.Net;
using System.Net.Sockets;

namespace FakeFurunoWcn;

public class ManagedClient
{
    public TcpClient? TcpClient { get; set; }
    public bool IsAntainnerSelected { get; set; }
}

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private List<ManagedClient> _clientList = new List<ManagedClient>();

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = WcnNotifyTask();
        var tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 10000);
        tcpListener.Start();
        while (!stoppingToken.IsCancellationRequested)
        {
            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // await Task.Delay(1000, stoppingToken);
            var client = await tcpListener.AcceptTcpClientAsync();
            var managedClient = new ManagedClient
            {
                TcpClient = client,
                IsAntainnerSelected = false,
            };
            lock (_clientList)
            {
                _clientList.Add(managedClient);
            }
            _ = ServiceMain(managedClient);
        }
    }

    private async Task ServiceMain(ManagedClient managedClient)
    {
        using (managedClient.TcpClient)
        {
            var ns = managedClient.TcpClient!.GetStream();
            byte[] buffer = new byte[1024];
            var recived = new List<byte>();
            var nRead = 0;
            while ((nRead = await ns.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                recived.AddRange(buffer.Take(nRead));

                var startIndex = recived.FindIndex(d => d == 0x31 || d == 0x06);
                if (startIndex < 0)
                {
                    continue;
                }
                recived = recived.Skip(startIndex).ToList();
                if (recived.Count < 4)
                {
                    continue;
                }

                var lenBytes = recived.Skip(2).Take(2).ToArray();
                if (BitConverter.IsLittleEndian)
                {
                    lenBytes = lenBytes.Reverse().ToArray();
                }
                var length = BitConverter.ToUInt16(lenBytes);

                var allLength = 4 + length + 1;
                if (recived.Count < allLength)
                {
                    continue;
                }

                var checkSum = CalcCheckSum(recived.Take(allLength - 1).ToArray());
                if (checkSum != recived[allLength - 1])
                {
                    _logger.LogError("CheckSum Error.");
                    recived = recived.Skip(allLength).ToList();
                    continue;
                }

                if (recived[0] == 0x31)
                {
                    managedClient.IsAntainnerSelected = true;
                    var response = new byte[] { 0x71, recived[1], 0x00, 0x01, recived[4] };
                    var sum = CalcCheckSum(response);
                    response = response.Append(sum).ToArray();
                    await ns.WriteAsync(response, 0, response.Length);
                }
                else if (recived[0] == 0x06)
                {
                    var response = recived.Take(allLength).ToArray();
                    await ns.WriteAsync(response, 0, response.Length);
                }

                recived = recived.Skip(allLength).ToList();
            }
        }
        lock (_clientList)
        {
            _clientList.Remove(managedClient);
        }
    }

    private async Task WcnNotifyTask()
    {
        while (true)
        {
            lock (_clientList)
            {
                var wcnNotify = new List<byte> { 0x90, 0, 0, 0x28 };
                wcnNotify.AddRange(System.Text.Encoding.ASCII.GetBytes("017988283474"));
                wcnNotify.AddRange(Enumerable.Repeat<byte>(0, 28));
                var bcc = CalcCheckSum(wcnNotify.ToArray());
                wcnNotify.Add(bcc);
                foreach (var managedClient in _clientList.Where(mc => mc.IsAntainnerSelected))
                {
                    managedClient.TcpClient!.GetStream().WriteAsync(wcnNotify.ToArray(), 0, wcnNotify.Count);
                }
            }
            await Task.Delay(1000);
        }
    }

    private byte CalcCheckSum(byte[] data)
    {
        var checkSum = 0;
        foreach (var d in data)
        {
            checkSum = checkSum ^ d;
        }

        return (byte)checkSum;
    }
}
