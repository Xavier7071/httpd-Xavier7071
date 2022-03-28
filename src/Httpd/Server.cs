using System.Net.Sockets;

namespace Httpd;

public class Server
{
    public Request? Request { get; set; }
    private readonly TcpListener _listener;
    private readonly bool _directoryListing;
    private readonly string[] _extensions;

    public Server(int port, bool directoryListing, string[] extensions)
    {
        _directoryListing = directoryListing;
        _extensions = extensions;
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

    public void Build404Response(Response response)
    {
        response.Build404();
        SendResponse(response);
    }

    public void SendResponse(Response response)
    {
        Request!.RespondToRequest(response.ResponseBytes!);
    }

    public void BuildDirectoryListing()
    {
        if (!_directoryListing || !FolderPathIsValid())
        {
            var response = new Response();
            Build404Response(response);
            SendResponse(response);
        }
        else
        {
            // Directory Listing ici
        }
    }

    public bool FilePathIsValid()
    {
        return File.Exists(Environment.CurrentDirectory + GetFilePath()) &&
               _extensions.Contains(Path.GetExtension(Environment.CurrentDirectory + GetFilePath()));
    }

    public bool IsFolder()
    {
        return !Path.HasExtension(Request!.Path);
    }

    private bool FolderPathIsValid()
    {
        return Directory.Exists(Environment.CurrentDirectory + Request!.Path);
    }

    private string? GetFilePath()
    {
        if (!Path.HasExtension(Request!.Path))
        {
            return Request.Path + "index.html";
        }

        return Request.Path;
    }
}