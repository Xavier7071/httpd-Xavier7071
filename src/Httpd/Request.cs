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
    public Dictionary<string, string> Dictionary { get; private set; }
    private NetworkStream? _stream;
    private Socket? _socket;
    private byte[]? _buffer;

    public Request(TcpClient client)
    {
        Dictionary = new Dictionary<string, string>();
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
        string?[] queryStringParameters  = ServerRequest!.Split();
        HttpMethod = queryStringParameters [0];
        Protocol = queryStringParameters [2];

        if (queryStringParameters [1]!.Contains('?'))
        {
            SetDictionary(queryStringParameters [1]!);
            var pathEnd = queryStringParameters [1]!.IndexOf('?');
            Path = queryStringParameters [1]![..pathEnd];
        }
        else
        {
            Path = queryStringParameters [1]!;
        }

        if (ServerRequest.Contains("gzip"))
        {
            AcceptsGzip = true;
        }
    }

    private void SetDictionary(string path)
    {
        var parametersIndex = path.IndexOf('?');
        var parameters = path[parametersIndex..];
        // https://stackoverflow.com/questions/659887/get-url-parameters-from-a-string-in-net
        Dictionary = parameters.TrimStart('?').Split(new[] {'&', ';'}, StringSplitOptions.RemoveEmptyEntries)
            .Select(parameter => parameter.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries))
            .GroupBy(parts => parts[0],
                parts => parts.Length > 2
                    ? string.Join("=", parts, 1, parts.Length - 1)
                    : (parts.Length > 1 ? parts[1] : ""))
            .ToDictionary(grouping => grouping.Key,
                grouping => string.Join(",", grouping));
    }
}