namespace ConsoleAppORIS;

public class Book
{
    public string Name { get; set; }
    public string Author { get; set; }
    public string Year { get; set; }

    public Book(string name, string author, string year)
    {
        Name = name;
        Author = author;
        Year = year;
    }
}