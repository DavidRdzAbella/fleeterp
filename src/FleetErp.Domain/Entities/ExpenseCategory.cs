using FleetErp.Domain.Common;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Catálogo de conceptos de gasto (casetas, viáticos, refacciones, multas…).
/// Cada empresa arma el suyo; el sistema no asume una lista fija.
/// </summary>
/// <remarks>
/// El combustible NO se captura aquí sino en <see cref="FuelLog"/>, que es la
/// única fuente de verdad de litros y costo de diésel. Separarlos evita duplicar
/// el gasto al totalizar y permite calcular rendimiento (km por litro).
/// </remarks>
public class ExpenseCategory : TenantEntity, ISoftDeletable
{
    private ExpenseCategory() { }

    public ExpenseCategory(string code, string name, bool isTripRelated = true)
    {
        Update(code, name, isTripRelated);
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gasto directo del viaje (entra al costo del flete) vs. gasto de estructura
    /// (renta de patio, administración) que no se prorratea por viaje.
    /// </summary>
    public bool IsTripRelated { get; private set; } = true;

    public bool IsActive { get; private set; } = true;

    public void Update(string code, string name, bool isTripRelated)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(code), "El código del concepto de gasto es obligatorio.");
        DomainException.Require(!string.IsNullOrWhiteSpace(name), "El nombre del concepto de gasto es obligatorio.");
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        IsTripRelated = isTripRelated;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
