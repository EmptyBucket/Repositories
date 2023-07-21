using Repositories.Abstractions.Repositories;

namespace Samples;

public class SecondEntity : IEntity<Guid, FirstEntity>
{
    private SecondEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public SecondEntity(string name) : this(Guid.NewGuid(), name)
    {
    }

    public Guid Id { get; }

    public string Name { get; }
}