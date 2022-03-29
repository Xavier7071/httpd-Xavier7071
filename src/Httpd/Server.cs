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
        else if (IsFolder(request.Path!) && _directoryListing && FolderPathIsValid(request))
        {
            request.RespondToRequest(BuildDirectoryListing(request));
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

    private static byte[] BuildDirectoryListing(Request request)
    {
        var response = new Response("null");
        var stringBuilder = new StringBuilder();
        
        stringBuilder.Append("<html><body style='text-align: center;'>\r\n");
        stringBuilder.Append("<h1>Index of " + request.Path + "</h1>\r\n");
        var folders = Directory.GetDirectories(Environment.CurrentDirectory + request.Path!);
        var files = Directory.GetFiles(Environment.CurrentDirectory + request.Path!);
        stringBuilder.Append("<table style='margin: auto;'><tr><th>Name</th><th>Last modified</th><th>Size(Bytes)</th></tr>\r\n");

        foreach (var folder in folders)
        {
            var folderInfo = new DirectoryInfo(folder);
            var substring = folder[(folder.LastIndexOf('/') + 1)..];
            stringBuilder.Append("<tr><th><a href='" + substring + "/'>" + substring + "</a></th><td>" + folderInfo.LastWriteTime +"</td></tr>\r\n");
        }

        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            var substring = file[(file.LastIndexOf('/') + 1)..];
            stringBuilder.Append("<tr><th><a href='" + substring + "'>" + substring + "</a></th><td>" + fileInfo.LastWriteTime +"</td><td>" + fileInfo.Length +"</td></tr>\r\n");
        }

        stringBuilder.Append("</table></body></html>\r\n");
        response.Build(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
        return response.ResponseBytes!;
    }

    private bool FilePathIsValid(Request request)
    {
        return File.Exists(Environment.CurrentDirectory + GetFilePath(request)) &&
               _extensions.Contains(Path.GetExtension(Environment.CurrentDirectory + GetFilePath(request)));
    }

    private static bool IsFolder(string path)
    {
        if (!path.EndsWith("/"))
        {
            return false;
        }
        return !Path.HasExtension(path);
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