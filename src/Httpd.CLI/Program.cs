using System.Text;
using Httpd;
using Httpd.CLI;

var config = new Config();
var server = new Server(config.Port, config.DirectoryListing, config.Extensions);

server.AddHandler("GET", "/debug", (request, response) =>
{
    var html = new StringBuilder();
    html.Append("<html><body><h1>Requete</h1>");
    html.Append(request.HttpMethod + "<br>");
    html.Append(request.Path + "<br>");
    html.Append(request.Protocol + "<br><br>");
    html.Append(request.ServerRequest);
    html.Append("<h1>Parametres de la requete</h1>");
    html.Append("<table><tr><th>Cle</th><th>Valeur</th></tr>");
    foreach (var (key, value) in request.Dictionary)
    {
        html.Append(@$"<tr><th>{key}</th><td>{value}</td></tr>");
    }

    html.Append("</table><h1>Reponse</h1>");
    foreach (var header in response.GetHeaders())
    {
        html.Append(header + "<br>");
    }

    html.Append("</body></html>\r\n");
    response.Build(Encoding.UTF8.GetBytes(html.ToString()), request.AcceptsGzip);
    request.RespondToRequest(response.ResponseBytes!);
});

await server.Start();