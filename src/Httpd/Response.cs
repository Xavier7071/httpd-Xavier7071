using System.Text;

namespace Httpd;

public class Response
{
    public byte[]? ResponseBytes { get; private set; }

    public void Build(string serverRequest)
    {
        var response = "HTTP/1.1 200 OK \r\n";
        response += "Content-Length: 1000\r\n";
        response += "Content-Type: text/html\r\n";
        response += "Connection: close\r\n";
        response += "\r\n";
        response += "<html><body><h1>It works!</h1><h1>It works!</h1>" + Environment.CurrentDirectory +
                    "<h1>It works!</h1><h1>It works!</h1></body></html>\r\n";

        ResponseBytes = Encoding.UTF8.GetBytes(response);
    }
}