using AuthAPI.Core;
using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.RequestStore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AuthAPI.Samples.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            AuthAPIConfiguration.Instance.ResponseStore = new MemoryCacheResponseStore();

            var authStore = Substitute.For<IAuthStore>();
            authStore.GetClientSecretById(Arg.Any<string>())
                     .Returns<string>("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ=");

            authStore.GetPasswordByUserName(Arg.Any<string>())
                     .Returns("123456");

            AuthAPIConfiguration.Instance.AuthStore = authStore;

        }
    }
}
