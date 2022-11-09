namespace OctaAutheticationMVC.Models
{
    public class OktaSettings
    {
        public string DomainUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GroupId { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class HipsoOktaSettings : OktaSettings
    { 
    }

    public class NsoOktaSettings : OktaSettings
    {
    }

}
