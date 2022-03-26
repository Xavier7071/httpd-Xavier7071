using System.Net.Sockets;
using System.Text;

namespace Httpd;

public class Request
{
    private NetworkStream? _stream;
    private Socket? _socket;
    private byte[]? _buffer;

    public string Read(TcpClient client)
    {
        Thread.Sleep(50);

        _stream = client.GetStream();
        _socket = _stream.Socket;
        _buffer = new byte[_socket.Available];

        _stream.Read(_buffer, 0, _buffer.Length);

        return Encoding.UTF8.GetString(_buffer);
    }

    public void RespondToRequest(byte[] responseBytes)
    {
        _socket!.Send(responseBytes);
    }
}