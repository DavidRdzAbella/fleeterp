using FluentValidation;
using FleetErp.Application.Contracts;

namespace FleetErp.Application.Validation;

/// <summary>
/// Validación de forma de la petición (obligatorios, rangos, coherencia entre
/// campos). Las reglas de negocio propiamente dichas viven en el dominio; aquí
/// solo se evita que llegue basura a los agregados.
/// </summary>
public sealed class CreateTripRequestValidator : AbstractValidator<CreateTripRequest>
{
    public CreateTripRequestValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty().WithMessage("Seleccione un conductor.");
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Seleccione una unidad.");
        RuleFor(x => x.Origin).NotEmpty().MaximumLength(150).WithMessage("Indique el origen del viaje.");
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(150).WithMessage("Indique el destino del viaje.");
        RuleFor(x => x.PlannedDistance).GreaterThanOrEqualTo(0).WithMessage("Los kilómetros por recorrer no pueden ser negativos.");
        RuleFor(x => x.InitialFuel).GreaterThanOrEqualTo(0).WithMessage("El combustible inicial no puede ser negativo.");
        RuleFor(x => x.CargoWeight).GreaterThanOrEqualTo(0).WithMessage("El peso de la carga no puede ser negativo.");
        RuleFor(x => x.FreightRevenue).GreaterThanOrEqualTo(0).WithMessage("El flete no puede ser negativo.");
        RuleFor(x => x.DriverPayRate!.Value).GreaterThanOrEqualTo(0).When(x => x.DriverPayRate.HasValue)
            .WithMessage("La tarifa del conductor no puede ser negativa.");
        RuleFor(x => x.ScheduledArrivalUtc!.Value)
            .GreaterThan(x => x.ScheduledDepartureUtc).When(x => x.ScheduledArrivalUtc.HasValue)
            .WithMessage("La llegada programada debe ser posterior a la salida.");
        RuleFor(x => x.TrailerId).NotEqual(x => x.VehicleId).When(x => x.TrailerId.HasValue)
            .WithMessage("El remolque no puede ser la misma unidad motriz.");
    }
}

public sealed class UpdateTripRequestValidator : AbstractValidator<UpdateTripRequest>
{
    public UpdateTripRequestValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.Origin).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PlannedDistance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InitialFuel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CargoWeight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FreightRevenue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DriverPayRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ScheduledArrivalUtc!.Value)
            .GreaterThan(x => x.ScheduledDepartureUtc).When(x => x.ScheduledArrivalUtc.HasValue);
    }
}

public sealed class DispatchTripRequestValidator : AbstractValidator<DispatchTripRequest>
{
    public DispatchTripRequestValidator()
    {
        RuleFor(x => x.OdometerStart).GreaterThanOrEqualTo(0).WithMessage("Capture el odómetro de salida.");
        RuleFor(x => x.InitialFuel!.Value).GreaterThanOrEqualTo(0).When(x => x.InitialFuel.HasValue);
    }
}

public sealed class CompleteTripRequestValidator : AbstractValidator<CompleteTripRequest>
{
    public CompleteTripRequestValidator()
    {
        RuleFor(x => x.OdometerEnd).GreaterThanOrEqualTo(0).WithMessage("Capture el odómetro de llegada.");
        RuleFor(x => x.FinalFuel!.Value).GreaterThanOrEqualTo(0).When(x => x.FinalFuel.HasValue);
        RuleFor(x => x.DriverHours!.Value).InclusiveBetween(0, 720).When(x => x.DriverHours.HasValue)
            .WithMessage("Las horas del conductor no son razonables para un viaje.");
    }
}

public sealed class CancelTripRequestValidator : AbstractValidator<CancelTripRequest>
{
    public CancelTripRequestValidator() =>
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(300).WithMessage("Indique el motivo de la cancelación.");
}
