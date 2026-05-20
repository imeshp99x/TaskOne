using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TaskManagement.API.Repositories;

namespace TaskManagement.API.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserRepository _userRepository;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IUserRepository userRepository)
            : base(options, logger, encoder)
        {
            _userRepository = userRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization header");

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]!);
                if (authHeader.Scheme != "Basic")
                    return AuthenticateResult.Fail("Unsupported authentication scheme");

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                if (credentials.Length != 2)
                    return AuthenticateResult.Fail("Invalid authorization header format");

                var username = credentials[0];
                var password = credentials[1];

                var user = await _userRepository.GetByUsernameAsync(username);
                if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return AuthenticateResult.Fail("Invalid username or password");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.GivenName, user.FullName)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid authorization header");
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.Headers.Append("WWW-Authenticate", "Basic realm=\"TaskManagement\"");
            return Task.CompletedTask;
        }
    }
}
