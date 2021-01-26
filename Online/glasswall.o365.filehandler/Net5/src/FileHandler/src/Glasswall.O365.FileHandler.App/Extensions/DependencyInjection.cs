using System;
using Glasswall.O365.FileHandler.App.Clients;
using Glasswall.O365.FileHandler.App.Models;
using Glasswall.O365.FileHandler.App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Glasswall.O365.FileHandler.App.Extensions
{
    public static class DependencyInjection
    {
        public static void ConfigureGlasswallServices(this IServiceCollection services, IConfigurationSection glasswallSection)
        {
            var glasswallSettings = new GlasswallSettings();
            glasswallSection.Bind(glasswallSettings);
            services.AddSingleton(glasswallSettings);

            services.AddRefitClient<IGlasswallClient>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(glasswallSettings.BaseUrl);
                    c.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", glasswallSettings.ApiKey);
                });

            services.AddScoped<IFileRebuilder, FileRebuilder>();
        }
    }
}
