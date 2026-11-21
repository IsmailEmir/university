using System.Collections.Generic;

namespace ConsoleAppORIS;

public class Author
{
    public string Name { get; set; } = "";
    public List<Book> Books { get; set; } = new();
}