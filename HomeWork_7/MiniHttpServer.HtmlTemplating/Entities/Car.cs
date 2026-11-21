namespace MiniHttpServer.HtmlTemplating.Entities;

public class Car
{
    public string Year { get; set; }
    public string Name { get; set; }
    public string Marka { get; set; }

    public Car(string year, string name, string marka)
    {
        Year = year;
        Name = name;
        Marka = marka;
    }
}