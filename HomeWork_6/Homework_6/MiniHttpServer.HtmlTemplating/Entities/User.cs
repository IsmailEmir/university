namespace Templater;

public class User
{
    public string Name { get; set; }
    public string Group { get;  set;}
    public int Age { get; set;}
    public bool Gender { get; set;}

    public User(string name, string group, int age, bool gender)
    {
        Name = name;
        Group = group;
        Age = age;
        Gender = gender;
    }
}