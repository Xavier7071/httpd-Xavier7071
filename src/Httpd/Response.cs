using System.Collections;
using System.Text;

namespace Httpd;

public class Response
{
    public byte[]? ResponseBytes { get; private set; }
    private byte[]? _contentBytes;
    private byte[]? _headerBytes;
    private readonly ArrayList _responseCode;
    private readonly ArrayList _responseHeaders;
    private readonly string _path;

    public Response(string path)
    {
        _responseCode = new ArrayList
        {
            200,
            "OK"
        };
        _responseHeaders = new ArrayList();
        _path = path;
    }

    public void Build(byte[] content)
    {
        _contentBytes = content;
        _headerBytes = BuildHeader();
        ResponseBytes = _headerBytes!.Concat(_contentBytes!).ToArray();
    }

    public void SetResponseCode(int code, string message)
    {
        _responseCode[0] = code;
        _responseCode[1] = message;
    }

    public void AddHeader(string header, string parameter)
    {
        _responseHeaders.Add(header + ": " + parameter);
    }

    private byte[] BuildHeader()
    {
        var header = new StringBuilder();
        header.Append("HTTP/1.1 " + _responseCode[0] + " " + _responseCode[1] + "\r\n");
        header.Append("Content-Length: " + _contentBytes!.Length + "\r\n");
        header.Append("Content-Type: " + GetContentType() + "\r\n");
        foreach (var responseHeader in (_responseHeaders))
        {
            header.Append(responseHeader + "\r\n");
        }

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