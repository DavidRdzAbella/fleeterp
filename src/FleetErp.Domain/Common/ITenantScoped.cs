namespace FleetErp.Domain.Common;

/// <summary>
/// Entidad que pertenece a una empresa (tenant). Habilita el aislamiento
/// multi-empresa mediante filtros globales de consulta, de modo que el mismo
/// despliegue sirva a varias empresas sin cambios de código.
/// </summary>
public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
