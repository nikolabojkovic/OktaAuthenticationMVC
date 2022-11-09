using OctaAutheticationMVC.Models;
using Okta.Idx.Sdk;

namespace OctaAutheticationMVC.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthenticationResponse> SignIn(CredentialsInputModel credentials, IIdxClient context)
        {
            var authnOptions = new AuthenticationOptions
            {
                Username = credentials.Username,
                Password = credentials.Password,
            };

            return await context.AuthenticateAsync(authnOptions).ConfigureAwait(false);
        }

        public async Task<bool> ValidateUser(string email)
        {
            return true;
        }
    }
}
