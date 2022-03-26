using Httpd;

var server = new Server(3000);
await ManageServer();

async Task ManageServer()
{
    while (true)
    {
        var client = await server.GetClient();

        var request = new Request();
        var serverRequest = request.Read(client);

        var response = new Response();
        response.Build(serverRequest);

        request.RespondToRequest(response.ResponseBytes!);
    }
}