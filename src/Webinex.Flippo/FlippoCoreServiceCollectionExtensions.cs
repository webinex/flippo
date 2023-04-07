using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Flippo
{
    public static class FlippoCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddFlippo(
            [NotNull] this IServiceCollection services,
            [NotNull] Action<IFlippoCoreConfiguration> configure)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = FlippoCoreConfiguration.GetOrCreate(services);
            configure(configuration);

            return services;
        }
    }
}