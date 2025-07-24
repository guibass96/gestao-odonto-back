using Gbarber.Application.Commands.Auth.SignUp;
using Gbarber.Application.Dtos.Account;
using Gbarber.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthResponse _authService;

        public LoginCommandHandler(IAuthResponse authService)
        {
            _authService = authService;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request);
            return result;
        }
    }
}
