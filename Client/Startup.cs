using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.IdentityModel.Tokens.Jwt;

namespace Client
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
            

            services.AddAuthentication(config =>
                {
                    // Check the cookie to see if we are authenticated
                    config.DefaultAuthenticateScheme = Constants.CookieAuthScheme;

                    // When signing in, give the cookie
                    config.DefaultSignInScheme = Constants.CookieAuthScheme;

                    // Check to see if user is allowed to do smth through OAuth
                    config.DefaultChallengeScheme = Constants.ServerAuthScheme;
                })
                .AddCookie(Constants.CookieAuthScheme)
                .AddOAuth(Constants.ServerAuthScheme, config =>
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "https://localhost:44321/oauth/login";
                    config.TokenEndpoint = "https://localhost:44321/oauth/token";

                    config.SaveTokens = true;

                    config.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            var accessToken = context.AccessToken;
                            var payload = accessToken.Split('.')[1];

                            // The length of a base64 encoded string is always a multiple of 4
                            var mod4 = payload.Length % 4;
                            if (mod4 > 0)
                            {
                                payload += new string('=', 4 - mod4);
                            }
                            var bytes = Convert.FromBase64String(payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JwtPayload.Deserialize(jsonPayload);

                            foreach (var claim in claims.Claims)
                            {
                                context.Identity.AddClaim(claim);
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();

            services.AddControllersWithViews();

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
