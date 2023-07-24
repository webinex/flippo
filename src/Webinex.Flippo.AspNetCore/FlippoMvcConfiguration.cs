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
        IFlippoMvcConfiguration UsePolicy([NotNull] string schema, [NotNull] string policy);

        IFlippoMvcConfiguration UseAllowAnonymousController();
    }

    internal class FlippoMvcConfiguration : IFlippoMvcConfiguration, IFlippoMvcSettings
    {
        private readonly IMvcBuilder _mvcBuilder;
        private bool _allowAnonymousController = false;

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

        public IFlippoMvcConfiguration UseAllowAnonymousController()
        {
            _allowAnonymousController = true;
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
            CodedResults.Success(string.Equals(reference, etag.Substring(1, etag.Length - 2), StringComparison.InvariantCultureIgnoreCase));

        internal void Complete()
        {
            _mvcBuilder.AddController(_allowAnonymousController
                ? typeof(AnonymousFlippoControllerBase)
                : typeof(FlippoController));
        }
    }
}