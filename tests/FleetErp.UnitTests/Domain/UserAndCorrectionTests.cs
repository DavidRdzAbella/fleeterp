using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;
using FluentAssertions;

namespace FleetErp.UnitTests.Domain;

public class AppUserTests
{
    private static AppUser NewUser() =>
        new("Admin@Demo.com", "Ana Ramírez", "hash", UserRole.Administrator);

    [Fact]
    public void El_correo_se_normaliza_a_minusculas()
    {
        NewUser().Email.Should().Be("admin@demo.com");
    }

    [Fact]
    public void Cambiar_el_correo_tambien_lo_normaliza()
    {
        var user = NewUser();

        user.ChangeEmail("  NUEVO@Demo.COM  ");

        user.Email.Should().Be("nuevo@demo.com");
    }

    [Fact]
    public void Un_correo_sin_arroba_se_rechaza()
    {
        var user = NewUser();

        var act = () => user.ChangeEmail("no-es-un-correo");

        act.Should().Throw<DomainException>().WithMessage("*formato válido*");
    }

    [Fact]
    public void El_nombre_no_puede_quedar_vacio()
    {
        var user = NewUser();

        var act = () => user.Rename("   ");

        act.Should().Throw<DomainException>().WithMessage("*nombre*");
    }

    [Fact]
    public void Registrar_entrada_guarda_la_fecha()
    {
        var user = NewUser();
        var now = DateTimeOffset.UtcNow;

        user.RegisterLogin(now);

        user.LastLoginUtc.Should().Be(now);
    }
}

/// <summary>
/// Correcciones sobre movimientos ya capturados: el usuario se equivoca al
/// teclear y el sistema tiene que dejarlo arreglar sin borrar y volver a crear.
/// </summary>
public class MovementCorrectionTests
{
    private static readonly DateTimeOffset When = new(2026, 3, 10, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Corregir_una_carga_recalcula_su_importe()
    {
        var log = new FuelLog(Guid.NewGuid(), When, 250m, 26m);
        log.TotalCost.Should().Be(6_500m);

        log.SetAmounts(300m, 25m);

        log.TotalCost.Should().Be(7_500m);
    }

    [Fact]
    public void Una_carga_se_puede_reasignar_a_otra_unidad_y_fecha()
    {
        var log = new FuelLog(Guid.NewGuid(), When, 250m, 26m);
        var otraUnidad = Guid.NewGuid();

        log.Reassign(otraUnidad, When.AddDays(1));

        log.VehicleId.Should().Be(otraUnidad);
        log.LoadedAtUtc.Should().Be(When.AddDays(1));
    }

    [Fact]
    public void Reasignar_una_carga_sin_unidad_se_rechaza()
    {
        var log = new FuelLog(Guid.NewGuid(), When, 250m, 26m);

        var act = () => log.Reassign(Guid.Empty, When);

        act.Should().Throw<DomainException>().WithMessage("*unidad*");
    }

    [Fact]
    public void Un_gasto_se_puede_recategorizar()
    {
        var expense = new Expense(Guid.NewGuid(), When, 1_800m, "Casetas");
        var otroConcepto = Guid.NewGuid();

        expense.Recategorize(otroConcepto, When.AddDays(-1));

        expense.CategoryId.Should().Be(otroConcepto);
        expense.IncurredAtUtc.Should().Be(When.AddDays(-1));
    }

    [Fact]
    public void Recategorizar_sin_concepto_se_rechaza()
    {
        var expense = new Expense(Guid.NewGuid(), When, 1_800m, "Casetas");

        var act = () => expense.Recategorize(Guid.Empty, When);

        act.Should().Throw<DomainException>().WithMessage("*concepto*");
    }
}
