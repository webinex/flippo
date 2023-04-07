using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

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

        internal void Complete()
        {
            _mvcBuilder.AddController(_allowAnonymousController ? typeof(AnonymousFlippoControllerBase) : typeof(FlippoController));
        }
    }
}