namespace FleetErp.Domain.Common;

/// <summary>
/// Violación de una invariante o de una transición de estado del negocio.
/// La API la traduce a HTTP 409/422; nunca a 500.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public static void Require(bool condition, string message)
    {
        if (!condition) throw new DomainException(message);
    }
}
