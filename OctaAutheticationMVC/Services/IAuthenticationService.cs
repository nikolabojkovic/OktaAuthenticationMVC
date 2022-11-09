using OctaAutheticationMVC.Models;
using Okta.Idx.Sdk;

namespace OctaAutheticationMVC.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> SignIn(CredentialsInputModel creadentials, IIdxClient context);

        Task<bool> ValidateUser(string email);
    }
}
