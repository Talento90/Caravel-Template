using System;
using Caravel.Entities;

namespace CaravelTemplate.Events;

public class CreateEntityEvent : IDomainEvent
{
    public Guid Id { get; }
    public string Name => $"{After.GetType().Name}Created";
    public DateTime EventDate { get; set; }
    public IEntity After { get; }

    public CreateEntityEvent(IEntity createdEntity)
    {
        Id = Guid.NewGuid();
        After = createdEntity;
        EventDate = DateTime.UtcNow;
    }
}