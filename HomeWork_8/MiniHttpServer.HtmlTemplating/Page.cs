namespace MiniHttpServer.HtmlTemplating;

public class Page
{
    public string Title { get; set; } = "";
    public string Header { get; set; } = "";
    public string Year { get; set; } = "";
    public string SiteName { get; set; } = "";
    public List<Item> Items { get; set; } = new();
}

public class Item
{
    public string Name { get; set; } = "";
    public string Price { get; set; } = "";
}