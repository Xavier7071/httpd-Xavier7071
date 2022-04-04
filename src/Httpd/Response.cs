using System.Collections;
using System.IO.Compression;
using System.Text;

namespace Httpd;

public class Response
{
    public byte[]? ResponseBytes { get; private set; }
    public ArrayList ResponseCode { get; }
    public string? ResponsePath { get; set; }
    private byte[]? _contentBytes;
    private byte[]? _headerBytes;
    private readonly ArrayList _responseHeaders;

    public Response(string? responsePath)
    {
        ResponseCode = new ArrayList
        {
            200,
            "OK"
        };
        _responseHeaders = new ArrayList();
        ResponsePath = responsePath;
    }

    public void Build(byte[] content, bool acceptsGZip)
    {
        if (acceptsGZip)
        {
            CompressContent(content);
        }
        else
        {
            _contentBytes = content;
        }

        _headerBytes = BuildHeader();
        ResponseBytes = _headerBytes!.Concat(_contentBytes!).ToArray();
    }

    public void SetResponseCode(int code, string message)
    {
        ResponseCode[0] = code;
        ResponseCode[1] = message;
    }

    public void AddHeader(string header, string parameter)
    {
        _responseHeaders.Add(header + ": " + parameter);
    }

    public List<string> GetHeaders()
    {
        var headers = new List<string>
        {
            "HTTP/1.1 " + ResponseCode[0] + " " + ResponseCode[1],
            "Content-Length: null",
            "Content-Type: text/html"
        };
        headers.AddRange(from object? responseHeader in (_responseHeaders) select responseHeader.ToString()!);
        headers.Add("Connection: close");

        return headers;
    }

    private void CompressContent(byte[] content)
    {
        AddHeader("Content-Encoding", "gzip");
        using var compressedStream = new MemoryStream();
        using var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);
        zipStream.Write(content, 0, content.Length);
        zipStream.Close();
        _contentBytes = compressedStream.ToArray();
    }

    private byte[] BuildHeader()
    {
        var header = new StringBuilder();
        header.Append("HTTP/1.1 " + ResponseCode[0] + " " + ResponseCode[1] + "\r\n");
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
        if (!File.Exists(Environment.CurrentDirectory + ResponsePath))
        {
            return "text/html";
        }

        return Path.GetExtension(Environment.CurrentDirectory + ResponsePath) switch
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