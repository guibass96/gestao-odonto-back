using Gbarber.Application.Dtos.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application.Commands.Auth.SignUp
{
    public class SighUpCommand : IRequest<AuthResponse>
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string Password { get; set; }


    }
}
