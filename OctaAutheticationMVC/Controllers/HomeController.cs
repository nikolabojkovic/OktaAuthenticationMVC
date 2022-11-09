using Microsoft.AspNetCore.Mvc;
using OctaAutheticationMVC.Models;
using Okta.Idx.Sdk;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;
using System.Diagnostics;

namespace OctaAutheticationMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IdxClient _idxClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _idxClient = new IdxClient(new IdxConfiguration()
            {
                Issuer = "https://dev-07584216.okta.com/oauth2/default",
                ClientId = "0oa6t304aaBnPTscA5d7",
                ClientSecret = "_vB3fZdSqPxpb67Juvb4SnRT9rNGaIt-uSm586qZ",
                RedirectUri = "http://localhost:8080/authorization-code/callback",
                Scopes = new List<string> { "openid", "profile", "offline_access" }
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("home/sigh-in")]
        public async Task<IActionResult> SignIn([FromBody] CredentialsInputModel credentials)
        {
            var authnOptions = new AuthenticationOptions
            {
                Username = credentials.Username,
                Password = credentials.Password,
            };

            try
            {
                var authnResponse = await _idxClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);

                switch (authnResponse?.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        return Ok(new { result = authnResponse.AuthenticationStatus.ToString() });

                    case AuthenticationStatus.PasswordExpired:
                    case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        BadRequest(new { error = "Invalid state" });
                        break;
                    default:
                        return Ok();
                }
            }
            catch (OktaException exception)
            {
                return BadRequest(new { error = $"Invalid login attempt: {exception.Message}" });
            }

            return Ok();
        }
    }
}