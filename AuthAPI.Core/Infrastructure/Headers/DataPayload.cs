namespace AuthAPI.Core.Infrastructure.Headers
{
    public class DataPayload
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ClientId { get; set; }

        public string RequestURI { get; set; }
        public string RequestBodyBase64 { get; set; }
        public string Method { get; set; }
    }
}
