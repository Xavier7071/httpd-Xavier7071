using Httpd;

var server = new Server(3000);
await ManageServer();

async Task ManageServer()
{
    while (true)
    {
        var client = await server.GetClient();
        var serverRequest = server.GetRequest(client);
        server.BuildResponse(serverRequest);
        server.SendResponse();
    }
}