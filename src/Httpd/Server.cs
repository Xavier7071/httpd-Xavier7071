using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace Httpd;

public class Server
{
    private readonly TcpListener _listener;
    private readonly bool _directoryListing;
    private readonly string[] _extensions;
    private readonly List<Route> _routes;

    private struct Route
    {
        public string HttpMethod;
        public string Path;
        public Action<Request, Response> Action;
    }

    public Server(int port, bool directoryListing, string[] extensions)
    {
        _routes = new List<Route>();
        _directoryListing = directoryListing;
        _extensions = extensions;
        _listener = TcpListener.Create(port);
        _listener.Start();
    }

    public async Task Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        while (true)
        {
            var client = await GetClient();
            new Thread(() =>
            {
                var timer = new Stopwatch();
                timer.Start();
                var request = HandleRequest(client);
                var response = new Response(GetFilePath(request)!);
                if (request.ServerRequest!.Length <= 0) return;
                HandleResponse(request, response);
                timer.Stop();
                Log.Information(
                    @$"{request.HttpMethod} {request.Path} {response.ResponseCode[0]} {timer.Elapsed.Milliseconds}ms - {response.ResponseBytes!.Length / 8}");
            }).Start();
        }
    }

    public void AddHandler(string httpMethod, string path, Action<Request, Response> action)
    {
        _routes.Add(new Route
        {
            HttpMethod = httpMethod,
            Path = path,
            Action = action
        });
    }

    private async Task<TcpClient> GetClient()
    {
        return await _listener.AcceptTcpClientAsync();
    }

    private static Request HandleRequest(TcpClient client)
    {
        var request = new Request(client);
        return request;
    }

    private void HandleResponse(Request request, Response response)
    {
        foreach (var route in _routes.Where(route =>
                     route.Path.Equals(request.Path) && route.HttpMethod.Equals(request.HttpMethod)))
        {
            RunRoute(route, request, response);
            return;
        }

        if (FilePathIsValid(request))
        {
            response.Build(File.ReadAllBytes(Environment.CurrentDirectory + GetFilePath(request)), request.AcceptsGzip);
            request.RespondToRequest(response.ResponseBytes!);
        }
        else if (IsFolder(request.Path!) && _directoryListing && FolderPathIsValid(request))
        {
            var directoryListing = new DirectoryListing(request, response);
            request.RespondToRequest(directoryListing.ResponseBytes);
        }
        else
        {
            var responseBytes = Build404Response(response);
            request.RespondToRequest(responseBytes);
        }
    }

    private static void RunRoute(Route route, Request request, Response response)
    {
        response.ResponsePath = null;
        route.Action.Invoke(request, response);
    }

    private static byte[] Build404Response(Response response)
    {
        response.ResponsePath = null;
        response.SetResponseCode(404, "NOT FOUND");
        response.Build(Encoding.UTF8.GetBytes(
                "<html><body><h1>Not Found</h1><h3>The requested URL was not found on this server.</h3></body></html>\r\n"),
            false);
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