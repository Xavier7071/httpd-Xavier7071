using System.Net.Sockets;
using System.Text;

namespace Httpd;

public class Server
{
    private TcpListener _listener;
    public int Port { get; set; }

    public Server(int port)
    {
        Port = port;
        _listener = TcpListener.Create(Port);
    }

    public async Task Start()
    {
        _listener.Start();
        Console.WriteLine($"Server has started on port {Port}!");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();

            HandleRequest(client);
        }
    }

    private void HandleRequest(TcpClient client)
    {
        Thread.Sleep(50);

        var stream = client.GetStream();
        var socket = stream.Socket;
        var buffer = new byte[socket.Available];

        stream.Read(buffer, 0, buffer.Length);

        //Console.WriteLine(BitConverter.ToString(buffer));
        var data = Encoding.UTF8.GetString(buffer);
        Console.WriteLine(data);

        var response = "HTTP/1.1 200 OK \r\n";
        response += "Content-Length: 44\r\n";
        response += "Content-Type: text/html\r\n";
        response += "Connection: close\r\n";
        response += "\r\n";
        response += "<html><body><h1>It works!</h1></body></html>\r\n";

        var responseBytes = Encoding.UTF8.GetBytes(response);
        socket.Send(responseBytes);
    }
}