using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthAPI.Tests.Integration
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication("AuthAPI")
                    .AddAuthAPIAuthentication<MockAuthStore>(o => { },
                                                             config =>
                                                             {
                                                                 config.TokenExpirationMiliseconds = (int)TimeSpan.FromMinutes(3).TotalMilliseconds;
                                                                 config.ClientId = "bXljbGllbnRpZA==";
                                                                 config.ClientSecret = "bXljbGllbnRzZWNyZXQ=";
                                                             });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
