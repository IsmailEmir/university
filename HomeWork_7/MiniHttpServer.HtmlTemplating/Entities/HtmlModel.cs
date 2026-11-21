namespace MiniHttpServer.HtmlTemplating.Entities;

public class HtmlModel
{
    public Page Page { get; set; } = new();
    public AuthenticatedUser User { get; set; } = new();
}