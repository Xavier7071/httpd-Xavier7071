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
        var request = Server.HandleRequest(client);
        if (request.ServerRequest!.Length <= 0) return;
        server.HandleResponse(request);
    }
}