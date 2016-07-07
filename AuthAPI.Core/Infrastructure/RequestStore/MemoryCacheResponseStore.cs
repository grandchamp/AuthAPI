using AuthAPI.Core.Infrastructure.Headers;
using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using AuthAPI.Core;

namespace AuthAPI.Core.Infrastructure.RequestStore
{
    public class MemoryCacheResponseStore : IResponseStore
    {
        private readonly MemoryCache _cache = MemoryCache.Default;

        public Task<bool> StoreResponse(ResponsePayload response, DateTime expirationDate)
        {
            var result = _cache.Add(response.Identifier, response.ToStoredResponse(expirationDate), expirationDate);

            return Task.FromResult(result);
        }

        public Task<bool> UpdateResponse(string identifier, ResponsePayload newResponse)
        {
            var currentCache = _cache.Remove(identifier);

            var result = false;
            if (currentCache != null)
            {
                var transformedRequest = (StoredResponse)currentCache;
                result = _cache.Add(identifier, newResponse.ToStoredResponse(transformedRequest.Expiration), transformedRequest.Expiration);
            }

            return Task.FromResult(result);
        }

        public Task<StoredResponse> GetResponse(string identifier)
        {
            var currentCache = _cache.Get(identifier);

            StoredResponse stored = null;
            if (currentCache != null)
                stored = (StoredResponse)currentCache;

            return Task.FromResult(stored);
        }
    }
}
