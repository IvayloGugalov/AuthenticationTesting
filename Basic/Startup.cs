using System.Collections.Generic;
using System.Security.Claims;
using Basic.Authorizations;
using Basic.Policies;
using Basic.PolicyProvider;
using Basic.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basic
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication("Auth")
                .AddCookie("Auth", config =>
                {
                    config.Cookie.Name = "IVO";
                    config.LoginPath = "/Home/Authenticate";

                    //config.Cookie.Name = "Monster";
                    //config.LoginPath = "/Operations/Authenticate";
                });

            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //    .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.Country)
                //    .Build();

                //config.DefaultPolicy = defaultAuthPolicy;

                // THESE ARE THE SAME

                config.AddPolicy("Claim.UserCountry", builder =>
                {
                    builder.RequireCustomClaim(ClaimTypes.Country);
                });

                config.AddPolicy(CookieJarPolicy.MonsterClaim, builder =>
                {
                    builder.RequireCustomClaim(CookieJarPolicy.CookieMonsterValue);
                });

            });

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, FirmHierarchyAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequirementClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>(_
                => new ClaimsTransformation(new List<Claim> { new Claim(ClaimTypes.Country, "Bulgaria") }));

            services.AddControllersWithViews(config =>
            {
                // Add global authorization for every controller
                //config.Filters.Add(new AuthorizeFilter());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRouting();

            // who you are?
            app.UseAuthentication();

            // are you allowed?
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
