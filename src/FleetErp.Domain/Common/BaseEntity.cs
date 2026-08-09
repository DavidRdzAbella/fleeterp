namespace FleetErp.Domain.Common;

/// <summary>
/// Raíz común de toda entidad persistente. Identidad por <see cref="Id"/>.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public override bool Equals(object? obj) =>
        obj is BaseEntity other && other.GetType() == GetType() && other.Id == Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
