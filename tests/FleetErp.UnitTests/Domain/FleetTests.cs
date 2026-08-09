using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;
using FluentAssertions;

namespace FleetErp.UnitTests.Domain;

public class VehicleTests
{
    private static Vehicle NewVehicle(decimal odometer = 100_000m)
    {
        var vehicle = new Vehicle("T-101", "AB-123-CD", Guid.NewGuid());
        vehicle.SetInitialOdometer(odometer);
        return vehicle;
    }

    [Fact]
    public void El_odometro_nunca_retrocede()
    {
        var vehicle = NewVehicle();

        var act = () => vehicle.UpdateOdometer(99_000m);

        act.Should().Throw<DomainException>().WithMessage("*no puede retroceder*");
    }

    [Fact]
    public void Una_unidad_en_taller_no_se_puede_mandar_a_viaje()
    {
        var vehicle = NewVehicle();
        vehicle.SendToMaintenance();

        var act = () => vehicle.MarkOnTrip();

        act.Should().Throw<DomainException>().WithMessage("*no está disponible*");
    }

    [Fact]
    public void Liberar_una_unidad_la_devuelve_a_disponible()
    {
        var vehicle = NewVehicle();
        vehicle.MarkOnTrip();

        vehicle.ReleaseFromTrip();

        vehicle.Status.Should().Be(VehicleStatus.Available);
    }

    [Fact]
    public void Una_unidad_fuera_de_servicio_puede_regresar_a_disponible()
    {
        var vehicle = NewVehicle();
        vehicle.SetOutOfService();

        vehicle.ReturnToService();

        vehicle.Status.Should().Be(VehicleStatus.Available);
    }

    [Fact]
    public void Una_unidad_en_taller_puede_regresar_a_disponible()
    {
        var vehicle = NewVehicle();
        vehicle.SendToMaintenance();

        vehicle.ReturnToService();

        vehicle.Status.Should().Be(VehicleStatus.Available);
    }

    [Fact]
    public void Una_unidad_en_viaje_no_se_libera_a_mano()
    {
        var vehicle = NewVehicle();
        vehicle.MarkOnTrip();

        var act = () => vehicle.ReturnToService();

        act.Should().Throw<DomainException>().WithMessage("*en viaje*");
    }

    [Fact]
    public void No_se_da_de_baja_una_unidad_que_anda_en_ruta()
    {
        var vehicle = NewVehicle();
        vehicle.MarkOnTrip();

        var act = () => vehicle.Deactivate();

        act.Should().Throw<DomainException>().WithMessage("*en viaje*");
    }

    [Fact]
    public void Un_ano_de_modelo_absurdo_se_rechaza()
    {
        var vehicle = NewVehicle();

        var act = () => vehicle.SetSpecs("Kenworth", "T680", 1800, null, null, null);

        act.Should().Throw<DomainException>().WithMessage("*año*");
    }
}

public class DriverTests
{
    private static Driver NewDriver() => new("Ulises", "Mendoza", "LIC-001");

    [Fact]
    public void El_porcentaje_sobre_el_flete_no_puede_pasar_de_cien()
    {
        var driver = NewDriver();

        var act = () => driver.SetCompensation(DriverPayScheme.PercentageOfRevenue, 120m);

        act.Should().Throw<DomainException>().WithMessage("*100*");
    }

    [Fact]
    public void Una_licencia_que_vence_dentro_del_umbral_se_marca()
    {
        var driver = NewDriver();
        var today = new DateOnly(2026, 3, 1);
        driver.SetLicense("LIC-001", "Federal E", today.AddDays(12));

        driver.LicenseExpiresWithin(today, 30).Should().BeTrue();
        driver.LicenseExpiresWithin(today, 5).Should().BeFalse();
    }

    [Fact]
    public void Un_conductor_en_viaje_no_se_da_de_baja()
    {
        var driver = NewDriver();
        driver.MarkOnTrip();

        var act = () => driver.Deactivate();

        act.Should().Throw<DomainException>().WithMessage("*en viaje*");
    }
}

public class FuelAndExpenseTests
{
    [Fact]
    public void El_importe_de_la_carga_es_cantidad_por_precio()
    {
        var log = new FuelLog(Guid.NewGuid(), DateTimeOffset.UtcNow, 250m, 25.90m);

        log.TotalCost.Should().Be(6_475m);
    }

    [Fact]
    public void Una_carga_sin_litros_no_tiene_sentido()
    {
        var act = () => new FuelLog(Guid.NewGuid(), DateTimeOffset.UtcNow, 0m, 25m);

        act.Should().Throw<DomainException>().WithMessage("*mayor a cero*");
    }

    [Fact]
    public void Un_gasto_exige_importe_positivo_y_descripcion()
    {
        var sinImporte = () => new Expense(Guid.NewGuid(), DateTimeOffset.UtcNow, 0m, "Casetas");
        var sinDescripcion = () => new Expense(Guid.NewGuid(), DateTimeOffset.UtcNow, 500m, " ");

        sinImporte.Should().Throw<DomainException>();
        sinDescripcion.Should().Throw<DomainException>();
    }
}

public class MaintenanceOrderTests
{
    private static readonly DateTimeOffset Opened = new(2026, 3, 1, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Cerrar_una_orden_registra_costo_y_taller()
    {
        var order = new MaintenanceOrder("OS-2026-000001", Guid.NewGuid(), MaintenanceKind.Preventive,
                                         Opened, "Servicio mayor");
        order.Start();

        order.Close(Opened.AddDays(2), 28_450m, "Taller Diésel Norte", 512_300m);

        order.Status.Should().Be(MaintenanceStatus.Closed);
        order.Cost.Should().Be(28_450m);
        order.Workshop.Should().Be("Taller Diésel Norte");
    }

    [Fact]
    public void El_cierre_no_puede_ser_anterior_a_la_apertura()
    {
        var order = new MaintenanceOrder("OS-2026-000001", Guid.NewGuid(), MaintenanceKind.Corrective,
                                         Opened, "Frenos");

        var act = () => order.Close(Opened.AddDays(-1), 1_000m, null, null);

        act.Should().Throw<DomainException>().WithMessage("*anterior a la apertura*");
    }
}

public class CustomFieldDefinitionTests
{
    [Fact]
    public void Un_campo_de_lista_exige_sus_opciones()
    {
        var act = () => new CustomFieldDefinition(CustomFieldTarget.Trip, "tipo", "Tipo", CustomFieldType.Select);

        act.Should().Throw<DomainException>().WithMessage("*opciones*");
    }

    [Fact]
    public void La_llave_se_normaliza_para_poder_guardarla_en_json()
    {
        var field = new CustomFieldDefinition(CustomFieldTarget.Trip, "Carta Porte", "Carta porte", CustomFieldType.Text);

        field.Key.Should().Be("carta_porte");
    }

    [Fact]
    public void Las_opciones_se_leen_separadas_por_barra()
    {
        var field = new CustomFieldDefinition(CustomFieldTarget.Trip, "tipo_carga", "Tipo de carga",
                                              CustomFieldType.Select, options: "General|Refrigerada|Granel");

        field.OptionList.Should().Equal("General", "Refrigerada", "Granel");
    }
}
