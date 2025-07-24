using GBarber.Core.Customers;
using GBarber.Core.Interfaces;
using MediatR;

namespace Gbarber.Application.Commands.Customer.AddCustomer;

public class CustomerCommandHandler : IRequestHandler<CustomerCommand, string>
{

    private readonly IRepositoryManager _repo;

    public CustomerCommandHandler(IRepositoryManager repo)
    {
        _repo = repo;
    }

    public async Task<string> Handle(CustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity()
        {
            //Id = Guid.NewGuid(),
            Name = request.Name,
            Cpf = request.Cpf,
            DtBirthday = request.DtBirthday,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,

        };
        await _repo.Customer.AddCustomer(customer);
        await _repo.SaveAsync();

        return "";
    }
}

