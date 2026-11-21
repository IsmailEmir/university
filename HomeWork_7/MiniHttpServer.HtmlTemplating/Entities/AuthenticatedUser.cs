namespace MiniHttpServer.HtmlTemplating.Entities;

public class AuthenticatedUser
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public bool IsAuthenticated { get; set; }
}