using Gbarber.Application.Dtos.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest request);
    }
}
