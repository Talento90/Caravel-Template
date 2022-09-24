using Caravel.Entities;

namespace CaravelTemplate.Entities;

public class Event : Entity
{
    public string Name { get; set; }
    public string Data { get; set; }

    public Event(string name, string data)
    {
        Data = data;
        Name = name;
    }
}