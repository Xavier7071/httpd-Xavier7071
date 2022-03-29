using System.Text;

namespace Httpd;

public class DirectoryListing
{
    public byte[] ResponseBytes { get; }
    private readonly StringBuilder _stringBuilder;
    private readonly Request _request;

    public DirectoryListing(Request request)
    {
        _request = request;
        var response = new Response("null");
        _stringBuilder = new StringBuilder();
        BuildHeader();
        BuildFolders();
        BuildFiles();
        BuildFooter();
        response.Build(Encoding.UTF8.GetBytes(_stringBuilder.ToString()));
        ResponseBytes = response.ResponseBytes!;
    }

    private void BuildHeader()
    {
        _stringBuilder.Append("<html><body style='text-align: center;'>\r\n");
        _stringBuilder.Append("<h1>Index of " + _request.Path + "</h1>\r\n");
        _stringBuilder.Append(
            "<table style='margin: auto;'><tr><th>Name</th><th>Last modified</th><th>Size(Bytes)</th></tr>\r\n");
    }

    private void BuildFolders()
    {
        var folders = Directory.GetDirectories(Environment.CurrentDirectory + _request.Path!);
        foreach (var folder in folders)
        {
            var folderInfo = new DirectoryInfo(folder);
            var substring = folder[(folder.LastIndexOf('/') + 1)..];
            _stringBuilder.Append("<tr style='text-align: center;'><th><a href='" + substring + "/'>" + substring +
                                  "</a></th><td>" + folderInfo.LastWriteTime.ToShortDateString() + "</td></tr>\r\n");
        }
    }

    private void BuildFiles()
    {
        var files = Directory.GetFiles(Environment.CurrentDirectory + _request.Path!);
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            var substring = file[(file.LastIndexOf('/') + 1)..];
            _stringBuilder.Append("<tr style='text-align: center;'><th><a href='" + substring + "'>" + substring +
                                  "</a></th><td>" + fileInfo.LastWriteTime.ToShortDateString() + "</td><td>" +
                                  fileInfo.Length + "</td></tr>\r\n");
        }
    }

    private void BuildFooter()
    {
        _stringBuilder.Append("</table></body></html>\r\n");
    }
}