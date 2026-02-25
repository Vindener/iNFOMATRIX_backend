namespace Infomatrix.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}
