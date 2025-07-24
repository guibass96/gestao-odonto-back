using Gbarber.Application.Commands.Auth.Login;
using Gbarber.Application.Commands.Auth.SignUp;
using Gbarber.Application.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application.Interfaces
{
    public interface IAuthResponse
    {
        //For signup logic
        Task<AuthResponse> SignUpAsync(SighUpCommand model, string orgin);

        //For login logic
        Task<AuthResponse> LoginAsync(LoginCommand model);

        //for addroles logic
        Task<string> AssignRolesAsync(AssignRolesDto model);

        //for checking if the sent token is valid
        Task<AuthResponse> RefreshTokenCheckAsync(string token);

        // for revoking refreshrokens
        Task<bool> RevokeTokenAsync(string token);

        Task<string> ConfirmEmailAsync(string userId, string code);
        Task<string> GenerateLinkResetPassword(string email);
        Task<string> ResetPassword(string userId, string code, string newPassword);
        Task<AuthResponse> ResendEmailConfirmation(string mail);

    }
}
