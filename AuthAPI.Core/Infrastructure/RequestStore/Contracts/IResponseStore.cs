using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure.RequestStore.Contracts
{
    public interface IResponseStore
    {
        Task<bool> StoreResponse(ResponsePayload response, DateTime expirationDate);
        Task<bool> UpdateResponse(string identifier, ResponsePayload newResponse);
        Task<StoredResponse> GetResponse(string identifier);
    }
}
