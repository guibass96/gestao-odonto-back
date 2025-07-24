using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application.Commands.Customer.AddCustomer;

    public class CustomerCommand : IRequest<string>
    {
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Cpf { get; set; }

    public string? PhoneNumber { get; set; }

    public string? DtBirthday { get; set; }
}

