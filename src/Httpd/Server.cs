using System.Net.Sockets;
using System.Text;

namespace Httpd;

public class Server
{
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

    public static Request HandleRequest(TcpClient client)
    {
        var request = new Request(client);
        return request;
    }

    public void HandleResponse(Request request)
    {
        if (FilePathIsValid(request))
        {
            var response = new Response(GetFilePath(request)!);
            response.Build(File.ReadAllBytes(Environment.CurrentDirectory + GetFilePath(request)));
            request.RespondToRequest(response.ResponseBytes!);
        }
        else if (IsFolder(request) && _directoryListing && FolderPathIsValid(request))
        {
            BuildDirectoryListing();
        }
        else
        {
            request.RespondToRequest(Build404Response());
        }
    }

    private static byte[] Build404Response()
    {
        var response = new Response("null");
        response.SetResponseCode(404, "NOT FOUND");
        response.Build(Encoding.UTF8.GetBytes(
            "<html><body><h1>Not Found</h1><h3>The requested URL was not found on this server.</h3></body></html>\r\n"));
        return response.ResponseBytes!;
    }

    private static void BuildDirectoryListing()
    {
        // Directory Listing ici
    }

    private bool FilePathIsValid(Request request)
    {
        return File.Exists(Environment.CurrentDirectory + GetFilePath(request)) &&
               _extensions.Contains(Path.GetExtension(Environment.CurrentDirectory + GetFilePath(request)));
    }

    private static bool IsFolder(Request request)
    {
        return !Path.HasExtension(request.Path);
    }

    private static bool FolderPathIsValid(Request request)
    {
        return Directory.Exists(Environment.CurrentDirectory + request.Path);
    }

    private static string? GetFilePath(Request request)
    {
        if (!Path.HasExtension(request.Path))
        {
            return request.Path + "index.html";
        }

        return request.Path;
    }
}