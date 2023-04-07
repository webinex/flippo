using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Flippo.Interceptors;
using Webinex.Flippo.Interceptors.Impls;

namespace Webinex.Flippo
{
    public interface IFlippoInterceptorsConfiguration
    {
        IFlippoCoreConfiguration Configuration { get; }

        IFlippoInterceptorsConfiguration Add([NotNull] Type interceptorType);
    }

    public interface IFlippoCoreConfiguration
    {
        [NotNull] IServiceCollection Services { get; }

        [NotNull] IDictionary<string, object> Values { get; }

        [NotNull] IFlippoInterceptorsConfiguration Interceptors { get; }

        IFlippoCoreConfiguration AddFileSystemBlob([NotNull] string basePath);
    }

    internal class FlippoCoreConfiguration : IFlippoCoreConfiguration, IFlippoInterceptorsConfiguration
    {
        private FlippoCoreConfiguration(IServiceCollection services)
        {
            Services = services;

            services
                .AddScoped<IFlippo, FlippoFacade>()
                .AddScoped<IFlippoInterceptable, FlippoInterceptable>();
        }

        public static FlippoCoreConfiguration GetOrCreate([NotNull] IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            var instance = (FlippoCoreConfiguration)services
                .SingleOrDefault(x => x.ServiceType == typeof(FlippoCoreConfiguration))
                ?.ImplementationInstance;

            if (instance != null)
                return instance;

            instance = new FlippoCoreConfiguration(services);
            services.AddSingleton(instance);
            return instance;
        }

        public IServiceCollection Services { get; }
        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();
        public IFlippoInterceptorsConfiguration Interceptors => this;

        public IFlippoCoreConfiguration AddFileSystemBlob(string basePath)
        {
            basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
            if (!Path.IsPathFullyQualified(basePath))
                throw new ArgumentException("Might be fully qualified path", nameof(basePath));

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            Services.AddSingleton(new FileSystemBlobSettings(basePath));
            Services.AddScoped<IFlippoBlobStore, FileSystemBlobStore>();
            return this;
        }

        public IFlippoCoreConfiguration Configuration => this;

        public IFlippoInterceptorsConfiguration Add(Type interceptorType)
        {
            interceptorType = interceptorType ?? throw new ArgumentNullException(nameof(interceptorType));

            if (!typeof(IFlippoInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException(
                    $"{interceptorType.Name} might be assignable to {nameof(IFlippoInterceptor)}");
            }

            Services.AddScoped(interceptorType);
            Services.Decorate(
                typeof(IFlippoInterceptable),
                typeof(FlippoInterceptorActivator<>).MakeGenericType(interceptorType));

            return this;
        }
    }

    public static class FlippoInterceptorsConfigurationExtensions
    {
        public static IFlippoInterceptorsConfiguration Add<T>(
            [NotNull] this IFlippoInterceptorsConfiguration configuration)
            where T : IFlippoInterceptor
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return configuration.Add(typeof(T));
        }

        public static IFlippoInterceptorsConfiguration AddMaxSize(
            [NotNull] this IFlippoInterceptorsConfiguration configuration,
            int maxSize)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            configuration.Configuration.Services.AddSingleton(new MaxSizeInterceptor.Settings(maxSize));
            return configuration.Add<MaxSizeInterceptor>();
        }
    }
}