using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core
{
    public class AuthAPIConfiguration
    {
        private static readonly Lazy<AuthAPIConfiguration> lazy = new Lazy<AuthAPIConfiguration>(() => new AuthAPIConfiguration());

        public static AuthAPIConfiguration Instance { get { return lazy.Value; } }

        public IAuthStore AuthStore { get; set; }
        public IResponseStore ResponseStore { get; set; }

        public int TokenExpirationMiliseconds { get; set; }

        public AuthAPIConfiguration()
        {
            TokenExpirationMiliseconds = 600000; //10min
        }
    }
}
