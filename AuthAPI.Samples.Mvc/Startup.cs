using AuthAPI.Core;
using AuthAPI.Middlewares.Mvc;
using AuthAPI.Samples.Mvc.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthAPI.Samples.Mvc
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                                       {
                                           options.DefaultAuthenticateScheme = AuthAPIAuthenticationOptions.DefaultScheme;
                                           options.DefaultChallengeScheme = AuthAPIAuthenticationOptions.DefaultScheme;
                                       })
                    .AddAuthAPIAuthentication<MockAuthStore>(options => { },
                                                             options => options.TokenExpirationMiliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
