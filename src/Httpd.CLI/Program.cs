using Httpd;

var server = new Server(3000);
await ManageServer();

async Task ManageServer()
{
    while (true)
    {
        var client = await server.GetClient();

        var request = new Request();
        var serverRequest = server.HandleRequest(client, request);

        var response = new Response();
        server.BuildResponse(serverRequest, response);

        server.SendResponse(request, response);
    }
}