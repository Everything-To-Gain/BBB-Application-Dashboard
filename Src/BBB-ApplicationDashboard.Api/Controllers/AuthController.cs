using BBB_ApplicationDashboard.Infrastructure.Exceptions.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers
{
    public class AuthController(
        IAuthService authService,
        IUserService userService,
        IJwtTokenService jwtTokenService
    ) : CustomControllerBase
    {
        [HttpGet("google-login-scalar")]
        public IActionResult GoogleLoginScalar() => Ok(authService.GetGoogleLoginURI().ToString());

        [HttpGet("google-login")]
        public IActionResult GoogleLogin() => Redirect(authService.GetGoogleLoginURI().ToString());

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallbackServer(
            [FromQuery] string code,
            [FromQuery] string? redirectUrl = null
        )
        {
            //! 1. Validate Google authentication
            var tokenData =
                await authService.ExchangeCodeForTokenAsync(code, redirectUrl)
                ?? throw new UserUnauthorizedException("Invalid google token!");

            var payload =
                await authService.GetPayload(tokenData)
                ?? throw new UserUnauthorizedException("Email is not verified!");

            //! 2. Find user
            User? user = await userService.FindUser(payload.Email);

            if (user is null)
            {
                user = new User() { Email = payload.Email, UserSource = Source.Internal };
                await userService.CreateUser(user);
            }

            //! 3. Delegate cookie and token creation to handlers
            int expirationDays = 1000;
            var token = jwtTokenService.GenerateJwtToken(expirationDays, user);

            //! 4. Set cookie
            Response.Cookies.Append(
                "BBBPartnersAuth",
                token,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(expirationDays),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                }
            );

            return Ok(
                new
                {
                    Status = "Authenticated",
                    Message = "User successfully logged in ✅",
                    Token = token,
                    UserName = payload.Name,
                }
            );
        }

        [HttpGet("microsoft-login-scalar")]
        public IActionResult MicrosoftLoginScalar()
        {
            return Ok(authService.GetMicrosoftLoginURI().ToString());
        }

        [HttpGet("microsoft-login")]
        public IActionResult MicrosoftLogin() =>
            Redirect(authService.GetMicrosoftLoginURI().ToString());

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("microsoft-callback")]
        public async Task<IActionResult> MicrosoftCallbackServer(
            [FromQuery] string code,
            [FromQuery] string? redirectUrl = null
        )
        {
            //! 1. Validate Microsoft authentication
            var tokenData =
                await authService.ExchangeMicrosoftCodeForTokenAsync(code, redirectUrl)
                ?? throw new UserUnauthorizedException("Invalid microsoft token!");

            var userInfo =
                await authService.GetMicrosoftUserInfoAsync(tokenData.AccessToken)
                ?? throw new UserUnauthorizedException("Failed to get user info!");

            //! 2. Find user by email
            string email =
                userInfo.Mail
                ?? userInfo.UserPrincipalName
                ?? throw new UserUnauthorizedException("Email not found in user info!");

            User? user = await userService.FindUser(email);

            if (user is null)
            {
                user = new User() { Email = email, UserSource = Source.Internal };
                await userService.CreateUser(user);
            }

            //! 3. Delegate cookie and token creation to handlers
            int expirationDays = 365250;
            var token = jwtTokenService.GenerateJwtToken(expirationDays, user);

            //! 4. Set cookie
            Response.Cookies.Append(
                "BBBPartnersAuth",
                token,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(expirationDays),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                }
            );

            return Ok(
                new
                {
                    Status = "Authenticated",
                    Message = "User successfully logged in ✅",
                    Token = token,
                    UserName = userInfo.DisplayName ?? email,
                }
            );
        }

        [Authorize]
        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            return Ok();
        }
    }
}
