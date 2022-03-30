using System.Text;
using Httpd;
using Httpd.CLI;

var config = new Config();
var server = new Server(config.Port, config.DirectoryListing, config.Extensions);
server.AddHandler("GET", "/debug", (request, response) =>
{
    var html = @$"<html><body>{request.ServerRequest}</body></html>\r\n";
    response.Build(Encoding.UTF8.GetBytes(html));
    request.RespondToRequest(response.ResponseBytes!);
});

await ManageServer();

async Task ManageServer()
{
    while (true)
    {
        var client = await server.GetClient();
        new Thread(() =>
        {
            var request = Server.HandleRequest(client);
            if (request.ServerRequest!.Length <= 0) return;
            server.HandleResponse(request);
        }).Start();
    }
}