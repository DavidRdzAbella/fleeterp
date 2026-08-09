using FluentValidation;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Validation;

public sealed class CreateVehicleRequestValidator : AbstractValidator<CreateVehicleRequest>
{
    public CreateVehicleRequestValidator()
    {
        RuleFor(x => x.EconomicNumber).NotEmpty().MaximumLength(30).WithMessage("Capture el número económico.");
        RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(20).WithMessage("Capture la placa.");
        RuleFor(x => x.VehicleTypeId).NotEmpty().WithMessage("Seleccione el tipo de unidad.");
        RuleFor(x => x.Year!.Value).InclusiveBetween(1950, 2100).When(x => x.Year.HasValue);
        RuleFor(x => x.InitialOdometer).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CargoCapacity!.Value).GreaterThanOrEqualTo(0).When(x => x.CargoCapacity.HasValue);
        RuleFor(x => x.TankCapacity!.Value).GreaterThanOrEqualTo(0).When(x => x.TankCapacity.HasValue);
    }
}

public sealed class UpdateVehicleRequestValidator : AbstractValidator<UpdateVehicleRequest>
{
    public UpdateVehicleRequestValidator()
    {
        RuleFor(x => x.EconomicNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.VehicleTypeId).NotEmpty();
        RuleFor(x => x.Year!.Value).InclusiveBetween(1950, 2100).When(x => x.Year.HasValue);
    }
}

public sealed class UpsertDriverRequestValidator : AbstractValidator<UpsertDriverRequest>
{
    public UpsertDriverRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(60).WithMessage("Capture el nombre del conductor.");
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(60).WithMessage("Capture los apellidos del conductor.");
        RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(40).WithMessage("Capture el número de licencia.");
        RuleFor(x => x.Email!).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.PayRate).GreaterThanOrEqualTo(0).WithMessage("La tarifa no puede ser negativa.");
        RuleFor(x => x.PayRate).LessThanOrEqualTo(100)
            .When(x => x.PayScheme == DriverPayScheme.PercentageOfRevenue)
            .WithMessage("El porcentaje sobre el flete no puede exceder 100.");
    }
}

public sealed class UpsertCustomerRequestValidator : AbstractValidator<UpsertCustomerRequest>
{
    public UpsertCustomerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithMessage("Capture el nombre del cliente.");
        RuleFor(x => x.Email!).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

public sealed class CreateFuelLogRequestValidator : AbstractValidator<CreateFuelLogRequest>
{
    public CreateFuelLogRequestValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Seleccione la unidad que cargó.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad cargada debe ser mayor a cero.");
        RuleFor(x => x.PricePerUnit).GreaterThanOrEqualTo(0).WithMessage("El precio por unidad no puede ser negativo.");
        RuleFor(x => x.OdometerReading!.Value).GreaterThanOrEqualTo(0).When(x => x.OdometerReading.HasValue);
    }
}

public sealed class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Seleccione el concepto de gasto.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El importe debe ser mayor a cero.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(250).WithMessage("Describa el gasto.");
    }
}

public sealed class CreateMaintenanceOrderRequestValidator : AbstractValidator<CreateMaintenanceOrderRequest>
{
    public CreateMaintenanceOrderRequestValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Seleccione la unidad.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500).WithMessage("Describa el servicio requerido.");
    }
}

public sealed class CloseMaintenanceOrderRequestValidator : AbstractValidator<CloseMaintenanceOrderRequest>
{
    public CloseMaintenanceOrderRequestValidator()
    {
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0).WithMessage("El costo no puede ser negativo.");
        RuleFor(x => x.OdometerAtService!.Value).GreaterThanOrEqualTo(0).When(x => x.OdometerAtService.HasValue);
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.TenantSlug).NotEmpty().WithMessage("Indique la empresa.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Capture un correo válido.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Capture la contraseña.");
    }
}

public sealed class UpsertVehicleTypeRequestValidator : AbstractValidator<UpsertVehicleTypeRequest>
{
    public UpsertVehicleTypeRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
    }
}

public sealed class UpsertExpenseCategoryRequestValidator : AbstractValidator<UpsertExpenseCategoryRequest>
{
    public UpsertExpenseCategoryRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
    }
}

public sealed class UpsertCustomFieldDefinitionRequestValidator : AbstractValidator<UpsertCustomFieldDefinitionRequest>
{
    public UpsertCustomFieldDefinitionRequestValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(40)
            .Matches("^[A-Za-z0-9_ ]+$").WithMessage("La llave solo admite letras, números, espacios y guion bajo.");
        RuleFor(x => x.Label).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Options).NotEmpty().When(x => x.Type == CustomFieldType.Select)
            .WithMessage("Un campo de tipo lista requiere sus opciones separadas por |.");
    }
}
