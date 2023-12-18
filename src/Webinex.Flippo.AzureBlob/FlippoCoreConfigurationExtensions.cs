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

            var settings = new AzureBlobSettings(connectionString, container);
            configuration.Services.AddScoped(_ => settings);
            configuration.Services.AddScoped<IFlippoBlobStore, AzureBlobStore>();

            return configuration;
        }
    }
}