namespace AuthAPI.Core
{
    public class AuthAPIConfiguration
    {
        public int TokenExpirationMiliseconds { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool UseIdentity { get; set; }
    }
}
