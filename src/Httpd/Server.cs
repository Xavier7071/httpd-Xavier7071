using System.Net.Sockets;

namespace Httpd;

public class Server
{
    public Request? Request { get; set; }
    private readonly Listener _listener;
    private int Port { get; }

    public Server(int port)
    {
        Port = port;
        _listener = new Listener(Port);
    }

    public async Task<TcpClient> GetClient()
    {
        return await _listener.ListenOnPort();
    }

    public void BuildResponse(Response response)
    {
        response.Build(GetFilePath());
    }

    public void SendResponse(Response response)
    {
        Request!.RespondToRequest(response.ResponseBytes!);
    }

    public bool FilePathIsValid()
    {
        return File.Exists(Environment.CurrentDirectory + GetFilePath());
    }

    private string GetFilePath()
    {
        var request = Request!.ServerRequest!.Split();
        return request[1].Equals("/") ? "/index.html" : request[1];
    }
}