using AuthAPI.Core;
using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.RequestStore;
using AuthAPI.Middlewares.WebAPI.Handlers;
using AuthAPI.Tests.WebAPI.Helpers;
using NSubstitute;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace AuthAPI.Tests.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            Type valuesControllerType = typeof(AuthAPI.Samples.WebAPI.Controllers.ValuesController);

            var config = new HttpConfiguration();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new AuthAPIHandler());

            AuthAPIConfiguration.Instance.ResponseStore = new MemoryCacheResponseStore();

            var authStore = Substitute.For<IAuthStore>();
            authStore.GetClientSecretById(Arg.Any<string>())
                     .Returns<string>("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ=");

            authStore.GetPasswordByUserName(Arg.Any<string>())
                     .Returns("123456");

            AuthAPIConfiguration.Instance.AuthStore = authStore;

            config.Services.Replace(typeof(IAssembliesResolver), new ExternalAssemblyResolver());

            appBuilder.UseWebApi(config);
        } 
    }
}
