using Caravel.Entities;

namespace CaravelTemplate.Entities;

public class Events : Entity
{
    public string Name { get; set; }
    public string Data { get; set; }

    public Events(string name, string data)
    {
        Data = data;
        Name = name;
    }
}