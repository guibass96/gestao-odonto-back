using Gbarber.Application.Commands.Auth.Login;
using Gbarber.Application.Commands.Auth.SignUp;
using Gbarber.Application.Dtos.Account;
using Gbarber.Application.Interfaces;
using Gbarber.Logging;
using GBarber.Core;
using GBarber.WebApi.Filter;
using GBarber.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace GBarber.WebApi.Controllers.Authentication
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IAuthResponse _authService;

        public AuthController(IAuthResponse authService, IMediator mediator)
        {
            _authService = authService;
            _mediator = mediator;
        }

        # region SetRefreshTokenInCookies

        private void SetRefreshTokenInCookies(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };

            //cookieOptionsExpires = DateTime.UtcNow.AddSeconds(cookieOptions.Timeout);

            Response.Cookies.Append("refreshTokenKey", refreshToken, cookieOptions);
        }

        #endregion

        #region SignUp Endpoint
        [AllowAnonymous]
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUpAsync([FromBody] SighUpCommand model)
        {
            if (!ModelState.IsValid)
            {
                var badRequestResponse = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Dados inválidos.",
                    Result = null
                };
                return BadRequest(badRequestResponse);
            }

            try
            {
                var origin = Request.Headers["origin"].ToString();

                var result = await _mediator.Send(model);

                if (!result.ISAuthenticated)
                {
                    var authFailedResponse = new ApiResponse<string>
                    {
                        Success = false,
                        Message = result.Message ?? "Falha na criação do usuário.",
                        Result = null
                    };
                    return BadRequest(authFailedResponse);
                }

                SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);

                var successResponse = new ApiResponse<object>
                {
                    Success = true,
                    Message = "Usuário criado com sucesso.",
                    Result = result
                };

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Falha ao cadastrar usuário", ex);

                var errorResponse = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Erro ao cadastrar usuário: " + ex.Message,
                    Result = null
                };

                return StatusCode(500, errorResponse);
            }
        }

        #endregion

        #region Login Endpoint
        [AllowAnonymous]

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginCommand model)
        {
            if (!ModelState.IsValid)
            {
                var badRequestResponse = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Dados inválidos.",
                    Result = null
                };
                return BadRequest(badRequestResponse);
            }

            try
            {
                var result = await _mediator.Send(model);

                if (!result.ISAuthenticated)
                {
                    var authFailedResponse = new ApiResponse<object>
                    {
                        Success = false,
                        Message = result.Message ?? "Falha na autenticação.",
                        Result = result
                    };
                    return BadRequest(authFailedResponse);
                }

                if (!string.IsNullOrEmpty(result.RefreshToken))
                {
                    SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);
                }

                var successResponse = new ApiResponse<object>
                {
                    Success = true,
                    Message = "Login realizado com sucesso.",
                    Result = result
                };

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Falha ao realizar login", ex);

                var errorResponse = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Erro ao realizar login: " + ex.Message,
                    Result = null
                };

                return StatusCode(500, errorResponse);
            }
        }

        #endregion

        #region AssignRole Endpoint

        [Authorize(Roles = "SuperAdmin")]
        [TypeFilter(typeof(FilterAuthorizationError))]
        [ServiceFilter(typeof(AuthorizationFilterAttribute))]
        [HttpPost("AddRole")]

        public async Task<IActionResult> AddRoleAsync(AssignRolesDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AssignRolesAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        #endregion

        #region RefreshTokenCheck Endpoint

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshTokenCheckAsync()
        {
            var refreshToken = Request.Cookies["refreshTokenKey"];

            var result = await _authService.RefreshTokenCheckAsync(refreshToken);

            if (!result.ISAuthenticated)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion

        #region RevokeTokenAsync

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeTokenAsync(RevokeToken model)
        {
            var refreshToken = model.Token ?? Request.Cookies["refreshTokenKey"];

            //check if there is no token
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Token is required");

            var result = await _authService.RevokeTokenAsync(refreshToken);

            //check if there is a problem with "result"
            //if (!result)
            //    return BadRequest("Token is Invalid");

            return Ok("Done Revoke");
        }

        #endregion

        [AllowAnonymous]
        #region ConfirmEmail
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            try
            {
                var result = await _authService.ConfirmEmailAsync(userId, code);

                if (result=="")
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = result ?? "Não foi possível confirmar o e-mail.",
                        Result = null
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = result,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Erro ao confirmar e-mail", ex);

                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Erro interno ao confirmar e-mail: " + ex.Message,
                    Result = null
                });
            }
        }
        #endregion



        [AllowAnonymous]

        [HttpGet("forgot-password")]
        public async Task<IActionResult> ForgotPasswordEmailAsync([FromQuery] string email)
        {
            try
            {
                var link = await _authService.GenerateLinkResetPassword(email);

                var response = new ApiResponse<string>
                {
                    Success = true,
                    Message = "Link de redefinição de senha gerado com sucesso.",
                    Result = link
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Falha ao resetar senha", ex);

                var response = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Erro ao gerar link de redefinição: " + ex.Message,
                    Result = null
                };

                return StatusCode(500, response);
            }
        }

        [AllowAnonymous]

        [HttpGet("reset-password")]
        public async Task<ApiResponse<string>> ResetPasswordAsync([FromQuery] string userId, [FromQuery] string code, [FromQuery] string newPassword)
        {
            var apiResponse = new ApiResponse<string>();

            try
            {

                var data = await _authService.ResetPassword(userId, code, newPassword);
                apiResponse.Success = true;
                apiResponse.Result = data;
                return apiResponse;
            }
            catch (Exception ex)
            {

                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Falha ao resetar senha", ex);
                return apiResponse;
            }

        }

        [AllowAnonymous]

        [HttpGet("resend-confirm-email")]
        public async Task<IActionResult> ResendEmailConfirmationAsync([FromQuery] string email)
        {
            try
            {
                var link = await _authService.ResendEmailConfirmation(email);

                var response = new ApiResponse<object>
                {
                    Success = true,
                    Message = "Link de redefinição de senha gerado com sucesso.",
                    Result = link
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Falha ao resetar senha", ex);

                var response = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Erro ao gerar link de redefinição: " + ex.Message,
                    Result = null
                };

                return StatusCode(500, response);
            }
        }

    }
}
