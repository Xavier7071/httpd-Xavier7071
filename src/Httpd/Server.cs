using System.Net.Sockets;

namespace Httpd;

public class Server
{
    private readonly Listener _listener;
    private int Port { get; }

    public Server(int port)
    {
        Port = port;
        _listener = new Listener(Port);
        _listener.Start();
    }

    public async Task<TcpClient> GetClient()
    {
        return await _listener.ListenOnPort();
    }
}