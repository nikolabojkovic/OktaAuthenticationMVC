using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OctaAutheticationMVC.Models;
using OctaAutheticationMVC.Services;
using Okta.Idx.Sdk;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;
using System.Diagnostics;

namespace OctaAutheticationMVC.Controllers
{
    public class HpsoController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IdxClient _idxClient;
        private readonly HipsoOktaSettings _settings;
        private readonly OktaSettings _oktaCommonSettings;
        private readonly IAuthenticationService _authenticationService;

        public HpsoController(ILogger<HomeController> logger,
                              IOptions<HipsoOktaSettings> options,
                              IOptions<OktaSettings> oktaCommonSettings,
                              IAuthenticationService authenticationService)
        {
            _logger = logger;
            _settings = options.Value;
            _oktaCommonSettings = oktaCommonSettings.Value;
            _authenticationService = authenticationService;
             _idxClient = new IdxClient(new IdxConfiguration()
            {
                Issuer = _oktaCommonSettings.DomainUrl,
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret,
                RedirectUri = _settings.RedirectUrl,
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

        public async Task<IActionResult> ValidateUser([FromBody]  string email)
        {
            return Ok();
        }

        [HttpPost("hpso/sigh-in")]
        public async Task<IActionResult> SignIn([FromBody] CredentialsInputModel credentials)
        {
            try
            {
                var authnResponse = await _authenticationService.SignIn(credentials, _idxClient);

                switch (authnResponse?.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        return Ok(new
                        {
                            result = authnResponse.AuthenticationStatus.ToString(),
                            redirectUrl = _settings.RedirectUrl
                        }); ;

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