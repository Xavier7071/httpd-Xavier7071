using Httpd;

var server = new Server(3000);
await ManageServer();

async Task ManageServer()
{
    while (true)
    {
        var client = await server.GetRequest();

        var request = new Request();
        request.HandleRequest(client);
    }
}