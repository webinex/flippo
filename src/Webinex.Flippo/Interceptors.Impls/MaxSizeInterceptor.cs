using System.Threading.Tasks;
using Webinex.Coded;

namespace Webinex.Flippo.Interceptors.Impls
{
    internal class MaxSizeInterceptor : FlippoInterceptor
    {
        private readonly Settings _settings;

        public MaxSizeInterceptor(Settings settings)
        {
            _settings = settings;
        }

        public override async Task<CodedResult<string>> OnStoreAsync(
            FlippoStoreArgs args,
            INext<FlippoStoreArgs, CodedResult<string>> next)
        {
            if (args.Stream.Length > _settings.Value)
            {
                return FlippoFailures.MaxSize(args.Stream.Length).ToResult<string>();
            }

            return await base.OnStoreAsync(args, next);
        }

        internal class Settings
        {
            public Settings(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}