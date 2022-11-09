using Microsoft.AspNetCore.Mvc;
using OctaAutheticationMVC.Models;
using Okta.Idx.Sdk;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;
using System.Diagnostics;

namespace OctaAutheticationMVC.Controllers
{
    public class NsoController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IdxClient _idxClient;

        public NsoController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _idxClient = new IdxClient(new IdxConfiguration()
            {
                Issuer = configuration["OKTA:DomainUrl"],
                ClientId = configuration["NSO_OKTA:ClientId"],
                ClientSecret = configuration["NSO_OKTA:ClientSecret"],
                RedirectUri = configuration["NSO_OKTA:RedirectUrl"],
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

        [HttpPost("nso/sigh-in")]
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