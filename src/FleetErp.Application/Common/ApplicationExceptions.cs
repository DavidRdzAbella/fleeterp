namespace FleetErp.Application.Common;

/// <summary>
/// El recurso solicitado no existe (o no pertenece a la empresa en sesión, que
/// para el cliente es indistinguible: no se filtra la existencia de datos ajenos).
/// </summary>
public sealed class NotFoundException(string entity, object key)
    : Exception($"No se encontró {entity} con identificador '{key}'.")
{
    public string Entity { get; } = entity;
    public object Key { get; } = key;
}

/// <summary>Conflicto con datos ya existentes (folio, placa o número económico duplicado).</summary>
public sealed class ConflictException(string message) : Exception(message);

/// <summary>Credenciales inválidas o sesión sin permiso sobre el recurso.</summary>
public sealed class UnauthorizedException(string message) : Exception(message);
