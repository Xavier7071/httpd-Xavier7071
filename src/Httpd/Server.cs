using System.Net.Sockets;

namespace Httpd;

public class Server
{
    private readonly Listener _listener;
    private Request? _request;
    private Response? _response;
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

    public string GetRequest(TcpClient client)
    {
        _request = new Request();
        return _request.Read(client);
    }

    public void BuildResponse(string serverRequest)
    {
        _response = new Response();
        _response.Build(serverRequest);
    }

    public void SendResponse()
    {
        _request!.RespondToRequest(_response!.ResponseBytes!);
    }
}