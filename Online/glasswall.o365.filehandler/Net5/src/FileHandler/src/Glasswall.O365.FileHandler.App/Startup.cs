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
using Glasswall.O365.FileHandler.Utils;
using Glasswall.O365.FileHandler.App.Extensions;
using Serilog;
using Microsoft.Extensions.Logging;

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
            ConfigureAppInsightsServices(services);

            ConfigureAuthenticationServices(services);

            services.ConfigureGlasswallServices(Configuration.GetSection("Glasswall"));

            ConfigureMvcServices(services);
        }

        private void ConfigureMvcServices(IServiceCollection services)
        {
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

        private void ConfigureAuthenticationServices(IServiceCollection services)
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
                    var logger = context.HttpContext.RequestServices.GetService<ILogger<Startup>>();
                    logger.LogInformation("Executing OnRedirectToIdentityProvider");
                    if (previous != null)
                    {
                        await previous(context);
                    }
                    FileHandlerActivationParameters fileHandlerActivation;
                    if (context.Request.IsFileHandlerActivationRequest(out fileHandlerActivation))
                    {
                        logger.LogInformation("IsFileHandlerActivationRequest:true");
                        logger.LogInformation("FileHandlerActivationParameters: {@ActivationParameters}", fileHandlerActivation);
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
        }

        private void ConfigureAppInsightsServices(IServiceCollection services)
        {
            if (!string.IsNullOrEmpty(Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")))
            {
                services.AddApplicationInsightsTelemetry();
            }
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

            app.UseSerilogRequestLogging();

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
