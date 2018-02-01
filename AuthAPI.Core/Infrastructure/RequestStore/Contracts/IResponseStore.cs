using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure.RequestStore.Contracts
{
    public interface IResponseStore
    {
        Task StoreResponse(ResponsePayload response, DateTime expirationDate);
        Task UpdateResponse(string identifier, ResponsePayload newResponse);
        Task<StoredResponse> GetResponse(string identifier);
    }
}
