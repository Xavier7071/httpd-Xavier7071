using System.Net.Sockets;
using Httpd;
using Httpd.CLI;

var config = new Config();
var server = new Server(config.Port, config.DirectoryListing, config.Extensions);
await ManageServer();

async Task ManageServer()
{
    while (true)
    {
        var client = await server.GetClient();
        HandleRequest(client);
    }
}

void HandleRequest(TcpClient client)
{
    server.Request = new Request(client);
    if (server.Request.ServerRequest!.Length <= 0) return;

    if (server.FilePathIsValid())
    {
        var response = new Response();
        server.BuildResponse(response);
        server.SendResponse(response);
    }
    else if (server.FolderPathIsValid())
    {
        server.BuildDirectoryListing();
    }
}