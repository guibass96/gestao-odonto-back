using Gbarber.Application.Dtos.Account;
using Gbarber.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application.Commands.Auth.SignUp
{
    public class SighUpCommandHandler : IRequestHandler<SighUpCommand, AuthResponse>
    {
        private readonly IAuthResponse _authService;

        public SighUpCommandHandler(IAuthResponse authService)
        {
            _authService = authService;
        }

        async Task<AuthResponse> IRequestHandler<SighUpCommand, AuthResponse>.Handle(SighUpCommand request, CancellationToken cancellationToken)
        {

            var result = await _authService.SignUpAsync(request, "");
            return result;
        }
    }
}
