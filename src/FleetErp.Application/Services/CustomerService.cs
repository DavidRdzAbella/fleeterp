using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Entities;

namespace FleetErp.Application.Services;

public interface ICustomerService
{
    Task<Guid> CreateAsync(UpsertCustomerRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpsertCustomerRequest request, CancellationToken ct = default);
    Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default);
}

public sealed class CustomerService(IUnitOfWork uow, ICurrentTenant tenant) : ICustomerService
{
    public async Task<Guid> CreateAsync(UpsertCustomerRequest request, CancellationToken ct = default)
    {
        var customer = new Customer(request.Name) { TenantId = tenant.TenantId };
        Apply(customer, request);

        await uow.Customers.AddAsync(customer, ct);
        await uow.SaveChangesAsync(ct);
        return customer.Id;
    }

    public async Task UpdateAsync(Guid id, UpsertCustomerRequest request, CancellationToken ct = default)
    {
        var customer = await Require(id, ct);
        customer.Rename(request.Name);
        Apply(customer, request);

        uow.Customers.Update(customer);
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default)
    {
        var customer = await Require(id, ct);
        if (active) customer.Activate(); else customer.Deactivate();
        uow.Customers.Update(customer);
        await uow.SaveChangesAsync(ct);
    }

    private static void Apply(Customer customer, UpsertCustomerRequest request)
    {
        customer.SetContact(request.TaxId, request.ContactName, request.Phone, request.Email, request.Address);
        customer.CustomFields.Replace(request.CustomFields);
    }

    private async Task<Customer> Require(Guid id, CancellationToken ct) =>
        await uow.Customers.GetByIdAsync(id, ct) ?? throw new NotFoundException("el cliente", id);
}
