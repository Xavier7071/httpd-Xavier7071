using System.Net.Sockets;
using Httpd;

var server = new Server(3000);
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
    else
    {
        // checker pour le directory listing
    }
}