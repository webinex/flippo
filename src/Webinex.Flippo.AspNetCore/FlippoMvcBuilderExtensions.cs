using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Flippo.AspNetCore
{
    public static class FlippoMvcBuilderExtensions
    {
        public static IMvcBuilder AddFlippoController(
            [NotNull] this IMvcBuilder mvcBuilder,
            Action<IFlippoMvcConfiguration> configure = null)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));

            var configuration = new FlippoMvcConfiguration(mvcBuilder);
            configure?.Invoke(configuration);
            configuration.Complete();

            return mvcBuilder;
        }
    }
}