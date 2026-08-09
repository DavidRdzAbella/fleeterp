using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;
using FluentAssertions;

namespace FleetErp.UnitTests.Domain;

/// <summary>
/// El viaje concentra las reglas que más caro cuestan si fallan: la máquina de
/// estados, la distancia real y el cálculo de la nómina del operador.
/// </summary>
public class TripTests
{
    private static readonly DateTimeOffset Departure = new(2026, 3, 10, 6, 0, 0, TimeSpan.Zero);

    private static Trip NewTrip(decimal plannedDistance = 900m)
    {
        var trip = new Trip("VJ-2026-000001", Guid.NewGuid(), Guid.NewGuid(),
                            "Monterrey", "Ciudad de México", Departure);
        trip.SetRoute("Monterrey", "Ciudad de México", plannedDistance);
        trip.SetSchedule(Departure, Departure.AddHours(14));
        return trip;
    }

    [Fact]
    public void Un_viaje_nuevo_nace_en_planeacion()
    {
        NewTrip().Status.Should().Be(TripStatus.Planned);
    }

    [Fact]
    public void Despachar_registra_salida_y_lo_pone_en_ruta()
    {
        var trip = NewTrip();

        trip.Dispatch(Departure, odometerStart: 100_000m, initialFuel: 400m);

        trip.Status.Should().Be(TripStatus.InProgress);
        trip.ActualDepartureUtc.Should().Be(Departure);
        trip.OdometerStart.Should().Be(100_000m);
        trip.InitialFuel.Should().Be(400m);
    }

    [Fact]
    public void No_se_puede_despachar_dos_veces()
    {
        var trip = NewTrip();
        trip.Dispatch(Departure, 100_000m);

        var act = () => trip.Dispatch(Departure, 100_000m);

        act.Should().Throw<DomainException>().WithMessage("*planeación*");
    }

    [Fact]
    public void No_se_puede_concluir_un_viaje_que_no_ha_salido()
    {
        var trip = NewTrip();

        var act = () => trip.Complete(Departure.AddHours(12), 100_900m, 80m, 12m);

        act.Should().Throw<DomainException>().WithMessage("*en ruta*");
    }

    [Fact]
    public void La_distancia_real_sale_del_odometro()
    {
        var trip = NewTrip(plannedDistance: 900m);
        trip.Dispatch(Departure, 100_000m);

        trip.Complete(Departure.AddHours(13), 100_925m, finalFuel: 60m, driverHours: 13m);

        trip.ActualDistance.Should().Be(925m);
        trip.EffectiveDistance.Should().Be(925m);
    }

    [Fact]
    public void Mientras_no_haya_odometro_de_llegada_se_usa_la_distancia_planeada()
    {
        var trip = NewTrip(plannedDistance: 900m);
        trip.Dispatch(Departure, 100_000m);

        trip.ActualDistance.Should().Be(0m);
        trip.EffectiveDistance.Should().Be(900m);
    }

    [Fact]
    public void El_odometro_de_llegada_no_puede_ser_menor_al_de_salida()
    {
        var trip = NewTrip();
        trip.Dispatch(Departure, 100_000m);

        var act = () => trip.Complete(Departure.AddHours(10), 99_500m, null, null);

        act.Should().Throw<DomainException>().WithMessage("*no puede ser menor*");
    }

    [Fact]
    public void La_llegada_no_puede_ser_anterior_a_la_salida()
    {
        var trip = NewTrip();
        trip.Dispatch(Departure, 100_000m);

        var act = () => trip.Complete(Departure.AddHours(-1), 100_500m, null, null);

        act.Should().Throw<DomainException>().WithMessage("*anterior a la de salida*");
    }

    [Fact]
    public void El_combustible_consumido_descuenta_lo_cargado_en_ruta()
    {
        var trip = NewTrip();
        trip.SetFuelPlan(initialFuel: 400m, refuelPlanned: true);
        trip.Dispatch(Departure, 100_000m);

        var log = new FuelLog(trip.VehicleId, Departure.AddHours(4), quantity: 250m, pricePerUnit: 26m);
        trip.AddFuelLog(log);
        trip.Complete(Departure.AddHours(13), 100_900m, finalFuel: 120m, driverHours: 13m);

        // Salió con 400, cargó 250 y llegó con 120: quemó 530.
        trip.FuelConsumed.Should().Be(530m);
        trip.FuelEfficiency.Should().Be(Math.Round(900m / 530m, 2));
    }

    [Fact]
    public void Sin_combustible_final_no_se_reporta_rendimiento()
    {
        var trip = NewTrip();
        trip.Dispatch(Departure, 100_000m);
        trip.Complete(Departure.AddHours(13), 100_900m, finalFuel: null, driverHours: 13m);

        trip.FuelConsumed.Should().BeNull();
        trip.FuelEfficiency.Should().BeNull();
    }

    [Theory]
    [InlineData(DriverPayScheme.PerHour, 100, 1300)]        // 13 horas × 100
    [InlineData(DriverPayScheme.PerKilometer, 3, 2700)]     // 900 km × 3
    [InlineData(DriverPayScheme.FixedPerTrip, 2500, 2500)]  // monto fijo
    [InlineData(DriverPayScheme.PercentageOfRevenue, 10, 3000)] // 10 % de 30 000
    public void El_pago_al_operador_depende_de_su_esquema(DriverPayScheme scheme, decimal rate, decimal expected)
    {
        var trip = NewTrip(plannedDistance: 900m);
        trip.SetCommercialTerms(freightRevenue: 30_000m, payScheme: scheme, payRate: rate);
        trip.Dispatch(Departure, 100_000m);
        trip.Complete(Departure.AddHours(13), 100_900m, finalFuel: 100m, driverHours: 13m);

        trip.DriverPayAmount.Should().Be(expected);
    }

    [Fact]
    public void Si_no_se_capturan_horas_se_toman_las_del_recorrido()
    {
        var trip = NewTrip();
        trip.SetCommercialTerms(30_000m, DriverPayScheme.PerHour, 100m);
        trip.Dispatch(Departure, 100_000m);

        trip.Complete(Departure.AddHours(8), 100_500m, null, driverHours: null);

        trip.DriverHours.Should().Be(8m);
        trip.DriverPayAmount.Should().Be(800m);
    }

    [Fact]
    public void La_utilidad_resta_combustible_gastos_y_nomina_al_flete()
    {
        var trip = NewTrip();
        trip.SetCommercialTerms(freightRevenue: 30_000m, payScheme: DriverPayScheme.FixedPerTrip, payRate: 2_500m);
        trip.Dispatch(Departure, 100_000m);

        trip.AddFuelLog(new FuelLog(trip.VehicleId, Departure.AddHours(2), 250m, 26m));  // 6 500
        trip.AddExpense(new Expense(Guid.NewGuid(), Departure.AddHours(3), 1_800m, "Casetas"));
        trip.Complete(Departure.AddHours(13), 100_900m, 100m, 13m);

        trip.FuelCost.Should().Be(6_500m);
        trip.OtherExpensesCost.Should().Be(1_800m);
        trip.TotalCost.Should().Be(10_800m);
        trip.Profit.Should().Be(19_200m);
        trip.ProfitMargin.Should().Be(64m);
    }

    [Fact]
    public void Llegar_despues_de_lo_comprometido_marca_el_viaje_como_tarde()
    {
        var trip = NewTrip();
        trip.Dispatch(Departure, 100_000m);

        trip.Complete(Departure.AddHours(16), 100_900m, null, null);

        trip.IsLate.Should().BeTrue();
    }

    [Fact]
    public void Un_viaje_concluido_ya_no_se_puede_cancelar()
    {
        var trip = NewTrip();
        trip.Dispatch(Departure, 100_000m);
        trip.Complete(Departure.AddHours(13), 100_900m, null, null);

        var act = () => trip.Cancel("Ya no se requiere");

        act.Should().Throw<DomainException>().WithMessage("*no se puede cancelar*");
    }

    [Fact]
    public void Cancelar_exige_motivo()
    {
        var trip = NewTrip();

        var act = () => trip.Cancel("  ");

        act.Should().Throw<DomainException>().WithMessage("*motivo*");
    }

    [Fact]
    public void Un_viaje_cancelado_no_admite_gastos()
    {
        var trip = NewTrip();
        trip.Cancel("El cliente reprogramó");

        var act = () => trip.AddExpense(new Expense(Guid.NewGuid(), Departure, 500m, "Casetas"));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void El_remolque_no_puede_ser_la_misma_unidad_motriz()
    {
        var trip = NewTrip();
        var vehicleId = Guid.NewGuid();

        var act = () => trip.SetAssignment(Guid.NewGuid(), vehicleId, vehicleId, null);

        act.Should().Throw<DomainException>().WithMessage("*misma unidad motriz*");
    }

    [Fact]
    public void No_se_puede_reasignar_un_viaje_que_ya_salio()
    {
        var trip = NewTrip();
        trip.Dispatch(Departure, 100_000m);

        var act = () => trip.SetAssignment(Guid.NewGuid(), Guid.NewGuid(), null, null);

        act.Should().Throw<DomainException>().WithMessage("*planeación*");
    }
}
