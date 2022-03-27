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

    public void SendResponse(Response response)
    {
        Request!.RespondToRequest(response.ResponseBytes!);
    }

    public void BuildDirectoryListing()
    {
        if (!_directoryListing)
        {
            return;
        }

        // Checker si c'est bien ou folder ou juste un fichier missing
        // Erreur 404
        // Directory Listing ici
        // New Response ?
    }

    public bool FilePathIsValid()
    {
        return File.Exists(Environment.CurrentDirectory + GetFilePath()) && _extensions.Contains(Path.GetExtension(Environment.CurrentDirectory + GetFilePath()));
    }

    private string? GetFilePath()
    {
        return Request!.Path!.Equals("/") ? "/index.html" : Request.Path;
    }
}