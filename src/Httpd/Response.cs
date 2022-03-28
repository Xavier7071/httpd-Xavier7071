using System.Text;

namespace Httpd;

public class Response
{
    public byte[]? ResponseBytes { get; private set; }
    private byte[]? _contentBytes;
    private byte[]? _headerBytes;
    private string? _path;

    public void Build(string? filePath)
    {
        _path = filePath;
        BuildContent();
        BuildHeader();

        ResponseBytes = _headerBytes!.Concat(_contentBytes!).ToArray();
    }

    public void Build404()
    {
        var content = new StringBuilder();
        content.Append(
            "<html><body><h1>Not Found</h1><h3>The requested URL was not found on this server.</h3></body></html>\r\n");
        _contentBytes = Encoding.UTF8.GetBytes(content.ToString());

        var header = new StringBuilder();
        header.Append("HTTP/1.1 404 Not Found \r\n");
        header.Append("Content-Length: " + _contentBytes!.Length + "\r\n");
        header.Append("Content-Type: text/html\r\n");
        header.Append("Connection: close\r\n");
        header.Append("\r\n");
        _headerBytes = Encoding.UTF8.GetBytes(header.ToString());

        ResponseBytes = _headerBytes!.Concat(_contentBytes!).ToArray();
    }

    private void BuildContent()
    {
        _contentBytes = File.ReadAllBytes(Environment.CurrentDirectory + _path);
    }

    private void BuildHeader()
    {
        var header = new StringBuilder();
        header.Append("HTTP/1.1 200 OK \r\n");
        header.Append("Content-Length: " + _contentBytes!.Length + "\r\n");
        header.Append("Content-Type: " + GetContentType() + "\r\n");
        header.Append("Connection: close\r\n");
        header.Append("\r\n");

        _headerBytes = Encoding.UTF8.GetBytes(header.ToString());
    }

    private string GetContentType()
    {
        return Path.GetExtension(Environment.CurrentDirectory + _path) switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".jpg" => "image/jpg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".mov" => "video/quicktime",
            _ => ""
        };
    }
}