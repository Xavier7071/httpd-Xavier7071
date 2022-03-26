using System.Net.Sockets;

namespace Httpd;

public class Listener
{
    private readonly TcpListener _listener;

    public Listener(int port)
    {
        _listener = TcpListener.Create(port);
    }

    public void Start()
    {
        _listener.Start();
    }

    public async Task<TcpClient> ListenOnPort()
    {
        return await _listener.AcceptTcpClientAsync();
    }
}