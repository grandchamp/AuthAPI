﻿using AuthAPI.Core.Infrastructure;
using NSubstitute;
using System.Threading.Tasks;

namespace AuthAPI.Samples.Mvc.Store
{
    public class MockAuthStore : IAuthStore
    {
        private readonly IAuthStore _authStore;
        public MockAuthStore()
        {
            var authStore = Substitute.For<IAuthStore>();
            authStore.GetClientSecretById(Arg.Any<string>())
                     .Returns("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ=");

            authStore.GetPasswordByUserName(Arg.Any<string>())
                     .Returns("123456");

            _authStore = authStore;
        }

        public Task<string> GetClientSecretById(string clientId) => _authStore.GetClientSecretById(clientId);

        public Task<string> GetPasswordByUserName(string userName) => _authStore.GetPasswordByUserName(userName);
    }
}
