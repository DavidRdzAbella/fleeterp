using FluentValidation;
using FleetErp.Application.Contracts;

namespace FleetErp.Application.Validation;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150)
            .WithMessage("Capture un correo válido.");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120)
            .WithMessage("Capture el nombre del usuario.");
        RuleFor(x => x.Password).SetValidator(new PasswordStrengthValidator());
    }
}

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150)
            .WithMessage("Capture un correo válido.");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120)
            .WithMessage("Capture el nombre del usuario.");
    }
}

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator() => RuleFor(x => x.Password).SetValidator(new PasswordStrengthValidator());
}

/// <summary>
/// Exigencia mínima de contraseña, en un solo lugar para que el alta y el
/// restablecimiento no puedan pedir cosas distintas.
/// </summary>
internal sealed class PasswordStrengthValidator : AbstractValidator<string>
{
    public PasswordStrengthValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Capture la contraseña.");
        RuleFor(x => x).MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
        RuleFor(x => x).Matches("[A-Za-z]").WithMessage("La contraseña debe incluir al menos una letra.");
        RuleFor(x => x).Matches("[0-9]").WithMessage("La contraseña debe incluir al menos un número.");
    }
}

public sealed class UpdateExpenseRequestValidator : AbstractValidator<UpdateExpenseRequest>
{
    public UpdateExpenseRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Seleccione el concepto de gasto.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El importe debe ser mayor a cero.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(250).WithMessage("Describa el gasto.");
    }
}

public sealed class UpdateFuelLogRequestValidator : AbstractValidator<UpdateFuelLogRequest>
{
    public UpdateFuelLogRequestValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Seleccione la unidad que cargó.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad cargada debe ser mayor a cero.");
        RuleFor(x => x.PricePerUnit).GreaterThanOrEqualTo(0).WithMessage("El precio por unidad no puede ser negativo.");
        RuleFor(x => x.OdometerReading!.Value).GreaterThanOrEqualTo(0).When(x => x.OdometerReading.HasValue);
    }
}
