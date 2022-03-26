using System.Net.Sockets;
using System.Text;

namespace Httpd;

public class Request
{
    public void HandleRequest(TcpClient client)
    {
        Thread.Sleep(50);

        var stream = client.GetStream();
        var socket = stream.Socket;
        var buffer = new byte[socket.Available];

        stream.Read(buffer, 0, buffer.Length);

        var data = Encoding.UTF8.GetString(buffer);
        Console.WriteLine(data);

        var response = "HTTP/1.1 200 OK \r\n";
        response += "Content-Length: 1000\r\n";
        response += "Content-Type: text/html\r\n";
        response += "Connection: close\r\n";
        response += "\r\n";
        response += "<html><body><h1>It works!</h1><h1>It works!</h1>" + Environment.CurrentDirectory +
                    "<h1>It works!</h1><h1>It works!</h1></body></html>\r\n";

        var responseBytes = Encoding.UTF8.GetBytes(response);
        socket.Send(responseBytes);
    }
}