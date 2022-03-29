using System.Net.Sockets;
using System.Text;

namespace Httpd;

public class Request
{
    public string? Path { get; private set; }
    public string? HttpMethod { get; private set; }
    public string? Protocol { get; private set; }
    public string? ServerRequest { get; private set; }
    public bool AcceptsGzip { get; private set; }
    private NetworkStream? _stream;
    private Socket? _socket;
    private byte[]? _buffer;

    public Request(TcpClient client)
    {
        HttpMethod = "";
        Protocol = "";
        Path = "";
        AcceptsGzip = false;
        Read(client);
        if (ServerRequest!.Length <= 0) return;
        DissectRequest();
    }

    public void RespondToRequest(byte[] responseBytes)
    {
        if (AcceptsGzip)
        {
            //var stream = new MemoryStream(responseBytes);
            //MemoryStream memoryStream;
            //using (memoryStream = new MemoryStream())
            //{
            //    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            //    {
            //        stream.CopyTo(gZipStream);
            //    }
            //}
        }

        _socket!.Send(responseBytes);
    }

    private void Read(TcpClient client)
    {
        Thread.Sleep(50);

        _stream = client.GetStream();
        _socket = _stream.Socket;
        _buffer = new byte[_socket.Available];

        _stream.Read(_buffer, 0, _buffer.Length);

        ServerRequest = Encoding.UTF8.GetString(_buffer);
    }

    private void DissectRequest()
    {
        string?[] request = ServerRequest!.Split();
        HttpMethod = request[0];
        Path = request[1];
        Protocol = request[2];

        if (ServerRequest.Contains("gzip"))
        {
            AcceptsGzip = true;
        }
    }
}