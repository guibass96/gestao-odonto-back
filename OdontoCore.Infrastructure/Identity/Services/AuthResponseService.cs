using Gbarber.Application.Commands.Auth.Login;
using Gbarber.Application.Commands.Auth.SignUp;
using Gbarber.Application.Dtos.Account;
using Gbarber.Application.Dtos.Email;
using Gbarber.Application.Exceptions;
using Gbarber.Application.Interfaces;
using GBarber.Core.Entities;
using GBarber.Core.Interfaces;
using GBarber.Core.Settings;
using GBarber.SQL.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace GBarber.Infrastructure.Identity.Services
{
    public class AuthResponseService : IAuthResponse
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailSender;
        private readonly ITenantService _tenantService;
        private readonly JWT _Jwt;

        public AuthResponseService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, IEmailService emailSender, ITenantService tenantService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _Jwt = jwt.Value;
            _emailSender = emailSender;
            _tenantService = tenantService;
        }

        #region create JWT

        // create JWT
        private async Task<JwtSecurityToken> CreateJwtAsync(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            //generate the symmetricSecurityKey by the s.key
            var symmetricSecurityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Secret));

            //generate the signingCredentials by symmetricSecurityKey
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            //define the  values that will be used to create JWT
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.validIssuer,
                audience: _Jwt.validAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_Jwt.expiresIn),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        #endregion create JWT

        #region Generate RefreshToken

        //Generate RefreshToken
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpireOn = DateTime.UtcNow.AddDays(10),
                CreateOn = DateTime.UtcNow
            };
        }

        #endregion Generate RefreshToken

        #region SignUp Method



        public async Task<string> GenerateLinkResetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                 throw new NotFoundException("Usuário não encontrado!");

            var resetPasswordUri = await SendResetEmail(user, "");
            string mailTemplate = $@"<!DOCTYPE html>
                                        <html lang=""pt-BR"">
                                          <head>
                                            <meta charset=""UTF-8"" />
                                            <title>Redefinição de Senha</title>
                                            <style>
                                              body {{
                                                font-family: Arial, sans-serif;
                                                background-color: #f4f4f4;
                                                margin: 0;
                                                padding: 0;
                                              }}
                                              .container {{
                                                max-width: 600px;
                                                margin: 40px auto;
                                                background-color: #ffffff;
                                                padding: 30px;
                                                border-radius: 8px;
                                                box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                                                text-align: center;
                                              }}
                                              .button {{
                                                background-color: #1976d2;
                                                color: white;
                                                padding: 14px 28px;
                                                text-decoration: none;
                                                font-weight: bold;
                                                border-radius: 5px;
                                                display: inline-block;
                                                margin-top: 20px;
                                              }}
                                              .footer {{
                                                margin-top: 30px;
                                                font-size: 12px;
                                                color: #888888;
                                              }}
                                            </style>
                                          </head>
                                          <body>
                                            <div class=""container"">
                                              <h2>Redefinição de Senha</h2>
                                              <p>
                                                Recebemos uma solicitação para redefinir a sua senha. <br />
                                                Para continuar, clique no botão abaixo:
                                              </p>
                                              <a href=""{resetPasswordUri}"" class=""button"">Redefinir Senha</a>
                                              <p class=""footer"">
                                                Se você não solicitou a redefinição, apenas ignore este e-mail. <br />
                                                Sua senha permanecerá a mesma.
                                              </p>
                                            </div>
                                          </body>
                                        </html>";


            await _emailSender.SendEmailAsync(new EmailRequest()
            {
                ToEmail = user.Email,
                Body = mailTemplate,
                Subject = "Redefinir senha."
            });

            return "Email enviado com sucesso!";
        }




        public async Task<AuthResponse> ResendEmailConfirmation(string mail)
        {
            var auth = new AuthResponse();
            try
            {
                var user = await _userManager.FindByEmailAsync(mail);
                if (user == null )
                {
                    auth.Message = "Usuário não encontrado";
                    return auth;
                }
                var verificationUri = await SendVerificationEmail(user, "https://localhost:7146");
                string mailTemplate = $"<!DOCTYPE html>\n<html lang=\"pt-BR\">\n  <head>\n    <meta charset=\"UTF-8\" />\n    <title>Confirmação de E-mail</title>\n    <style>\n      body {{\n        font-family: Arial, sans-serif;\n        background-color: #f4f4f4;\n        margin: 0;\n        padding: 0;\n      }}\n      .container {{\n        max-width: 600px;\n        margin: 40px auto;\n        background-color: #ffffff;\n        padding: 30px;\n        border-radius: 8px;\n        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);\n        text-align: center;\n      }}\n      .button {{\n        background-color: #1976d2;\n        color: white;\n        padding: 14px 28px;\n        text-decoration: none;\n        font-weight: bold;\n        border-radius: 5px;\n        display: inline-block;\n        margin-top: 20px;\n      }}\n      .footer {{\n        margin-top: 30px;\n        font-size: 12px;\n        color: #888888;\n      }}\n    </style>\n  </head>\n  <body>\n    <div class=\"container\">\n      <h2>Bem-vindo(a) à nossa plataforma!</h2>\n      <p>\n        Estamos felizes por ter você com a gente. <br />\n        Para começar a usar todos os recursos, confirme seu e-mail clicando no botão abaixo:\n      </p>\n      <a href=\"{verificationUri}\" class=\"button\">Confirmar E-mail</a>\n      <p class=\"footer\">\n        Se você não se cadastrou em nossa plataforma, pode ignorar este e-mail.\n      </p>\n    </div>\n  </body>\n</html>\n";
                await _emailSender.SendEmailAsync(new EmailRequest()
                {
                    ToEmail = user.Email,
                    Body = mailTemplate,
                    Subject = "Confirm Registration"
                });
            }
            catch (Exception erros)
            {

                return new AuthResponse { Message = erros.ToString() };
            }


            return auth;
        }

        //SignUp
        public async Task<AuthResponse> SignUpAsync(SighUpCommand model, string orgin)
        {
            var auth = new AuthResponse();

            var userEmail = await _userManager.FindByEmailAsync(model.Email);
            var userName = await _userManager.FindByNameAsync(model.Username);

            //checking the Email and username
            if (userEmail is not null)
                return new AuthResponse { Message = "O Email já está em uso! " };

            if (userName is not null)
                return new AuthResponse { Message = "Username is Already used ! " };

            //fill
            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Cpf = ""
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            //check result
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}, ";
                }

                return new AuthResponse { Message = errors };
            }
             var tenant = await _tenantService.CreateTenantAsync("teste",user);
            //assign role to user by default
            await _userManager.AddToRoleAsync(user, "USER");

            #region SendVerificationEmail

            var verificationUri = await SendVerificationEmail(user, "https://localhost:7146");
            string mailTemplate = $"<!DOCTYPE html>\n<html lang=\"pt-BR\">\n  <head>\n    <meta charset=\"UTF-8\" />\n    <title>Confirmação de E-mail</title>\n    <style>\n      body {{\n        font-family: Arial, sans-serif;\n        background-color: #f4f4f4;\n        margin: 0;\n        padding: 0;\n      }}\n      .container {{\n        max-width: 600px;\n        margin: 40px auto;\n        background-color: #ffffff;\n        padding: 30px;\n        border-radius: 8px;\n        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);\n        text-align: center;\n      }}\n      .button {{\n        background-color: #1976d2;\n        color: white;\n        padding: 14px 28px;\n        text-decoration: none;\n        font-weight: bold;\n        border-radius: 5px;\n        display: inline-block;\n        margin-top: 20px;\n      }}\n      .footer {{\n        margin-top: 30px;\n        font-size: 12px;\n        color: #888888;\n      }}\n    </style>\n  </head>\n  <body>\n    <div class=\"container\">\n      <h2>Bem-vindo(a) à nossa plataforma!</h2>\n      <p>\n        Estamos felizes por ter você com a gente. <br />\n        Para começar a usar todos os recursos, confirme seu e-mail clicando no botão abaixo:\n      </p>\n      <a href=\"{verificationUri}\" class=\"button\">Confirmar E-mail</a>\n      <p class=\"footer\">\n        Se você não se cadastrou em nossa plataforma, pode ignorar este e-mail.\n      </p>\n    </div>\n  </body>\n</html>\n";
            await _emailSender.SendEmailAsync(new EmailRequest()
            {
                ToEmail = user.Email,
                Body = mailTemplate,
                Subject = "Confirm Registration"
            });

            #endregion SendVerificationEmail

            var jwtSecurityToken = await CreateJwtAsync(user);

            auth.Email = user.Email;
            auth.Roles = new List<string> { "User" };
            auth.ISAuthenticated = true;
            auth.UserName = user.UserName;
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            auth.TokenExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
            auth.Message = "SignUp Succeeded";

            // create new refresh token
            var newRefreshToken = GenerateRefreshToken();
            auth.RefreshToken = newRefreshToken.Token;
            auth.RefreshTokenExpiration = newRefreshToken.ExpireOn;

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            return auth;
        }

        #endregion SignUp Method

        #region Login Method

        //login
        public async Task<AuthResponse> LoginAsync(LoginCommand model)
        {
            var auth = new AuthResponse();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                auth.Message = "E-mail ou senha incorretos";
                return auth;
            }
       
            var userpass = await _userManager.CheckPasswordAsync(user, model.Password);
            if (user == null || !userpass)
            {
                auth.Message = "E-mail ou senha incorretos";
                return auth;
            }
            var mailCheck = await _userManager.IsEmailConfirmedAsync(user);

            if (!mailCheck)
            {
                auth.Message = "Confirme seu e-mail para continuar";
                auth.isEmailConfirmed = false;
                return auth;
            }
         

            var jwtSecurityToken = await CreateJwtAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            auth.Email = user.Email;

            auth.Roles = roles.ToList();
            auth.ISAuthenticated = true;
            auth.UserName = user.UserName;
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            auth.TokenExpiresOn = jwtSecurityToken.ValidTo;
            auth.Message = "Login Succeeded ";
            auth.Name = user.FirstName;

            //check if the user has any active refresh token
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                auth.RefreshToken = activeRefreshToken.Token;
                auth.RefreshTokenExpiration = activeRefreshToken.ExpireOn;
            }
            else
            //in case user has no active refresh token
            {
                var newRefreshToken = GenerateRefreshToken();
                auth.RefreshToken = newRefreshToken.Token;
                auth.RefreshTokenExpiration = newRefreshToken.ExpireOn;

                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
            }

            return auth;
        }

        #endregion Login Method

        #region Assign Roles Method





        //Assign Roles
        public async Task<string> AssignRolesAsync(AssignRolesDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var role = await _roleManager.RoleExistsAsync(model.Role);


            //check the user Id and role
            if (user == null || !role)
                return "Invalid ID or Role";

            //check if user is already assiged to selected role
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            //check result
            if (!result.Succeeded)
                return "Something went wrong ";

            return string.Empty;
        }

        #endregion Assign Roles Method

        #region check Refresh Tokens method

        //check Refresh Tokens
        public async Task<AuthResponse> RefreshTokenCheckAsync(string token)
        {
            var auth = new AuthResponse();

            //find the user that match the sent refresh token
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                auth.Message = "Invalid Token";
                return auth;
            }

            // check if the refreshtoken is active
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                auth.Message = "Inactive Token";
                return auth;
            }

            //revoke the sent Refresh Tokens
            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtSecurityToken = await CreateJwtAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            auth.Email = user.Email;
            auth.Roles = roles.ToList();
            auth.ISAuthenticated = true;
            auth.UserName = user.UserName;
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            auth.TokenExpiresOn = jwtSecurityToken.ValidTo;
            auth.RefreshToken = newRefreshToken.Token;
            auth.RefreshTokenExpiration = newRefreshToken.ExpireOn;

            return auth;
        }

        #endregion check Refresh Tokens method

        #region revoke Refresh Tokens method

        //revoke Refresh token
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            // check if the refreshtoken is active
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            //revoke the sent Refresh Tokens
            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();

            await _userManager.UpdateAsync(user);

            return true;
        }

        #endregion revoke Refresh Tokens method

        #region SendVerificationEmail

        private async Task<string> SendVerificationEmail(AppUser user, string origin)
        {
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/Auth/confirm-email/";

            //var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var _enpointUri = "http://localhost:3000/validateEmail";
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }

        private async Task<string> SendResetEmail(AppUser user, string origin)
        {
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/Auth/reset-password/";

            //var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var _enpointUri = "http://localhost:3000/reset-password";
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }
        public async Task<string> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user is null)
            {
                return "Usuário não existe";
            }
            if (user.EmailConfirmed)
            {
                return  $"A conta já foi confirmada {user.Email} com sucesso ";
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                       
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return $"A conta foi confirmada {user.Email} com sucesso ";
            }
            else
            {
                throw new Exception($"An error occured while confirming {user.Email}.");
            }
        }

        public async Task<string> ResetPassword(string userId, string code,string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("Usuário não encontrado!");
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
           
            var result = await  _userManager.ResetPasswordAsync(user, code, newPassword); 
            if (result.Succeeded)
            {
                return "Senha resetada com sucesso!";
            }
            else
            {
                throw new Exception($"Ocorreu um erro durante o reset da senha. {user.Email}.");
            }
        }

        #endregion SendVerificationEmail
    }
}
