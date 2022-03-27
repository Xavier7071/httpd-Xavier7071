using System.Net.Sockets;

namespace Httpd;

public class Server
{
    public Request? Request { get; set; }
    private readonly TcpListener _listener;

    public Server(int port)
    {
        _listener = TcpListener.Create(port);
        _listener.Start();
    }

    public async Task<TcpClient> GetClient()
    {
        return await _listener.AcceptTcpClientAsync();
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