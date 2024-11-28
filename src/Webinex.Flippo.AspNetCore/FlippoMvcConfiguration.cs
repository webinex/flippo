using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Webinex.Coded;

namespace Webinex.Flippo.AspNetCore
{
    public interface IFlippoMvcConfiguration
    {
        /// <summary>
        /// Uses provided schema and policy in <see cref="IFlippoMvcSettings"/> for authorization.
        /// Will be ignored when <see cref="UseAllowAnonymousController"/> is called
        /// </summary>
        IFlippoMvcConfiguration UsePolicy([NotNull] string schema, [NotNull] string policy);

        /// <summary>
        /// Registers custom controller. Controller must be inherited from <see cref="FlippoControllerBase"/>
        /// </summary>
        IFlippoMvcConfiguration UseController(Type controllerType);

        /// <summary>
        /// Registers custom controller. Controller must be derived from <see cref="FlippoControllerBase"/>
        /// </summary>
        IFlippoMvcConfiguration UseController<TController>() where TController : FlippoControllerBase;

        /// <summary>
        /// Registers anonymous controller, authorization checks will be skipped
        /// </summary>
        IFlippoMvcConfiguration UseAllowAnonymousController();
    }

    internal class FlippoMvcConfiguration : IFlippoMvcConfiguration, IFlippoMvcSettings
    {
        private readonly IMvcBuilder _mvcBuilder;
        private Type _controllerType = typeof(FlippoController);

        public FlippoMvcConfiguration(IMvcBuilder mvcBuilder)
        {
            _mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));
            _mvcBuilder.Services.AddSingleton<IFlippoMvcSettings>(this);
        }

        public IFlippoMvcConfiguration UsePolicy(string schema, string policy)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            return this;
        }

        public IFlippoMvcConfiguration UseController(Type controllerType)
        {
            if (!controllerType.IsAssignableTo(typeof(FlippoControllerBase)))
                throw new ArgumentException($"Controller must be derived from {nameof(FlippoControllerBase)}");

            _controllerType = controllerType;
            return this;
        }

        public IFlippoMvcConfiguration UseController<TController>() where TController : FlippoControllerBase
        {
            _controllerType = typeof(TController);
            return this;
        }

        public IFlippoMvcConfiguration UseAllowAnonymousController()
        {
            _controllerType = typeof(AnonymousFlippoControllerBase);
            return this;
        }

        public string Schema { get; private set; }
        public string Policy { get; private set; }
        public string ParamCookiePrefix { get; set; } = "__FLIPPO_";

        public Func<HttpContext, string, int, CodedResult<CacheControlHeaderValue>> CacheControl { get; set; } =
            (_, content, cacheForSeconds) => CodedResults.Success(new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(cacheForSeconds),
                Private = true,
            });

        public Func<HttpContext, string, CodedResult<EntityTagHeaderValue>> ETag { get; set; } = (_, reference) =>
            CodedResults.Success(new EntityTagHeaderValue("\"" + reference.ToLowerInvariant() + "\""));

        public Func<HttpContext, string, string, CodedResult<bool>> ETagValid { get; set; } = (_, reference, etag) =>
            CodedResults.Success(string.Equals(reference, etag.Substring(1, etag.Length - 2),
                StringComparison.InvariantCultureIgnoreCase));

        internal void Complete()
        {
            _mvcBuilder.AddController(_controllerType);
        }
    }
}