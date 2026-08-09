using FleetErp.Domain.Enums;

namespace FleetErp.Application.Contracts;

public sealed record VehicleTypeDto(Guid Id, string Code, string Name, VehicleCategory Category, bool IsActive);

public sealed record UpsertVehicleTypeRequest(string Code, string Name, VehicleCategory Category);

public sealed record ExpenseCategoryDto(Guid Id, string Code, string Name, bool IsTripRelated, bool IsActive);

public sealed record UpsertExpenseCategoryRequest(string Code, string Name, bool IsTripRelated);

public sealed record CustomFieldDefinitionDto(
    Guid Id, CustomFieldTarget Target, string Key, string Label, CustomFieldType Type,
    bool IsRequired, IReadOnlyList<string> Options, int DisplayOrder, bool IsActive);

public sealed record UpsertCustomFieldDefinitionRequest(
    CustomFieldTarget Target, string Key, string Label, CustomFieldType Type,
    bool IsRequired, string? Options, int DisplayOrder);

/// <summary>Par id/etiqueta para poblar combos sin traer la entidad completa.</summary>
public sealed record LookupItemDto(Guid Id, string Label, string? Detail = null);
