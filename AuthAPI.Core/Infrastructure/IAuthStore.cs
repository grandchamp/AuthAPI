using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure
{
    public interface IAuthStore
    {
        Task<string> GetClientSecretById(string clientId);
        Task<string> GetPasswordByUserName(string userName);
    }
}
