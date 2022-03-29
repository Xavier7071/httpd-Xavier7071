using System.Text;

namespace Httpd;

public class Response
{
    public byte[]? ResponseBytes { get; private set; }
    private byte[]? _contentBytes;
    private byte[]? _headerBytes;
    //private string[] _headers;
    private readonly string _path;

    public Response(string path)
    {
        _path = path;
        //_headers = new string[];
    }

    public void Build(byte[] content)
    {
        _contentBytes = content;
        _headerBytes = BuildHeader();
        ResponseBytes = _headerBytes!.Concat(_contentBytes!).ToArray();
    }

    private byte[] BuildHeader()
    {
        var header = new StringBuilder();
        header.Append("HTTP/1.1 200 OK \r\n");
        header.Append("Content-Length: " + _contentBytes!.Length + "\r\n");
        header.Append("Content-Type: " + GetContentType() + "\r\n");
        header.Append("Connection: close\r\n");
        header.Append("\r\n");

        return Encoding.UTF8.GetBytes(header.ToString());
    }

    private string GetContentType()
    {
        if (!File.Exists(Environment.CurrentDirectory + _path))
        {
            return "text/html";
        }

        return Path.GetExtension(Environment.CurrentDirectory + _path) switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".jpg" => "image/jpg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".mov" => "video/quicktime",
            _ => "text/html"
        };
    }
}