using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Flippo.AzureBlob
{
    public static class FlippoCoreConfigurationExtensions
    {
        public static IFlippoCoreConfiguration AddAzureBlob(
            [NotNull] this IFlippoCoreConfiguration configuration,
            [NotNull] string connectionString,
            [NotNull] string container)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            container = container ?? throw new ArgumentNullException(nameof(container));

            return configuration
                .AddAzureBlob(e =>
                {
                    e.ConnectionString = connectionString;
                    e.Container = container;
                });
        }

        public static IFlippoCoreConfiguration AddAzureBlob(
            [NotNull] this IFlippoCoreConfiguration configuration,
            [NotNull] Action<AzureBlobSettings> configure)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            configuration.Services
                .AddOptions<AzureBlobSettings>()
                .Configure(configure);
            configuration.Services.AddScoped<IFlippoBlobStore, AzureBlobStore>();

            return configuration;
        }
    }
}