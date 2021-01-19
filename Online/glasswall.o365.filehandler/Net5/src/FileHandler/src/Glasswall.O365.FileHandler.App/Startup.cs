using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Glasswall.O365.FileHandler.Models;
using Glasswall.O365.FileHandler.Controllers;
using Glasswall.O365.FileHandler.Utils;
using System;
using Glasswall.O365.FileHandler.App.Models;
using Microsoft.Graph;
using Glasswall.O365.FileHandler.App.Services;
using Refit;
using Glasswall.O365.FileHandler.App.Clients;
using Glasswall.O365.FileHandler.App.Extensions;

namespace Glasswall.O365.FileHandler
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
            
            var initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                        .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
                        .AddDistributedTokenCaches();
            services.AddDistributedMemoryCache();

            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                var previous = options.Events.OnRedirectToIdentityProvider;

                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    if (previous != null)
                    {
                        await previous(context);
                    }
                    FileHandlerActivationParameters fileHandlerActivation;
                    if (context.Request.IsFileHandlerActivationRequest(out fileHandlerActivation))
                    {
                        context.ProtocolMessage.LoginHint = fileHandlerActivation.UserId;
                        context.ProtocolMessage.DomainHint = "organizations";
                        CookieStorage.Save(context.Request.Form, context.Response);
                    }

                    var challengeProperties = context.Properties;
                    if (null != challengeProperties && challengeProperties.Items.ContainsKey("prompt"))
                    {
                        context.ProtocolMessage.Prompt = challengeProperties.Items["prompt"];
                    }
                };
            });

            services.AddGlasswall(Configuration.GetSection("Glasswall"));

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddRazorPages()
                 .AddMicrosoftIdentityUI();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
