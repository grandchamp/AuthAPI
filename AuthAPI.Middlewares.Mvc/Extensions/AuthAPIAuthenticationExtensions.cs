using AuthAPI.Core;
using AuthAPI.Core.Builders;
using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.RequestStore;
using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using AuthAPI.Middlewares.Mvc;
using Microsoft.AspNetCore.Authentication;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthAPIAuthenticationExtensions
    {
        public static AuthenticationBuilder AddAuthAPIAuthentication<TAuthStore>(this AuthenticationBuilder builder,
                                                                                 Action<AuthAPIAuthenticationOptions> configureOptions, 
                                                                                 Action<AuthAPIConfiguration> authApiConfiguration)
            where TAuthStore : class, IAuthStore
        {
            builder.Services.AddMemoryCache();

            return builder.AddAuthAPIAuthentication<TAuthStore, MemoryCacheResponseStore>(configureOptions, authApiConfiguration);
        }

        public static AuthenticationBuilder AddAuthAPIAuthentication<TAuthStore, TResponseStore>(this AuthenticationBuilder builder,
                                                                                                 Action<AuthAPIAuthenticationOptions> configureOptions,
                                                                                                 Action<AuthAPIConfiguration> authApiConfiguration)
            where TAuthStore : class, IAuthStore
            where TResponseStore : class, IResponseStore
        {
            builder.Services.Configure(authApiConfiguration);

            builder.Services.AddScoped<IAuthStore, TAuthStore>();
            builder.Services.AddScoped<IResponseStore, TResponseStore>();
            builder.Services.AddScoped<IPrincipalBuilder, PrincipalBuilder>();

            return builder.AddScheme<AuthAPIAuthenticationOptions, AuthAPIAuthenticationHandler>("AuthAPI", "AuthAPI", configureOptions);
        }

        public static AuthenticationBuilder UseIdentity(this AuthenticationBuilder builder)
        {
            builder.Services.PostConfigure<AuthAPIConfiguration>(options => options.UseIdentity = true);

            return builder;
        }
    }
}
