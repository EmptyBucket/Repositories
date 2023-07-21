using Repositories.Abstractions.Repositories;

namespace Samples;

public class FirstEntity : IEntity<Guid, FirstEntity>
{
    private FirstEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public FirstEntity(string name) : this(Guid.NewGuid(), name)
    {
    }

    public Guid Id { get; }

    public string Name { get; }
}