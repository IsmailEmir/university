namespace MiniHttpServer.HtmlTemplating.Entities;

public class Bag
{
    public double Volume { get; set; }
    public Brand Brand { get; set; } = new Brand();
}