using AuthAPI.Core.Infrastructure.Headers;
using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure.RequestStore
{
    public class MemoryCacheResponseStore : IResponseStore
    {
        private readonly IMemoryCache _cache;
        public MemoryCacheResponseStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task StoreResponse(ResponsePayload response, DateTime expirationDate)
        {
            await _cache.GetOrCreateAsync(response.Identifier, item =>
            {
                item.SetAbsoluteExpiration(expirationDate);

                return Task.FromResult(response.ToStoredResponse(expirationDate));
            });
        }

        public async Task UpdateResponse(string identifier, ResponsePayload newResponse)
        {
            var result = _cache.TryGetValue(identifier, out StoredResponse storedResponse);

            if (result)
            {
                _cache.Remove(identifier);

                await _cache.GetOrCreateAsync(identifier, item =>
                {
                    item.SetAbsoluteExpiration(storedResponse.Expiration);

                    return Task.FromResult(newResponse.ToStoredResponse(storedResponse.Expiration));
                });
            }
        }

        public Task<StoredResponse> GetResponse(string identifier) => Task.FromResult(_cache.Get<StoredResponse>(identifier));
    }
}
