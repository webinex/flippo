using System;
using System.Threading;
using System.Threading.Tasks;
using Webinex.Coded;

namespace Webinex.Flippo
{
    internal class FlippoFacade : IFlippo
    {
        private readonly IFlippoInterceptable _interceptable;

        public FlippoFacade(IFlippoInterceptable interceptable)
        {
            _interceptable = interceptable;
        }

        public Task<CodedResult<string>> StoreAsync(FlippoStoreArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return _interceptable.StoreAsync(args, cancel);
        }

        public Task<CodedResult> DeleteAsync(FlippoDeleteArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return _interceptable.DeleteAsync(args, cancel);
        }

        public Task<CodedResult<FlippoContent>> GetAsync(FlippoGetArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return _interceptable.GetAsync(args, cancel);
        }

        public Task<CodedResult<bool>> ExistsAsync(FlippoExistsArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return _interceptable.ExistsAsync(args, cancel);
        }

        public Task<CodedResult> CopyAsync(FlippoCopyArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return _interceptable.CopyAsync(args, cancel);
        }

        public Task<CodedResult> MoveAsync(FlippoMoveArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return _interceptable.MoveAsync(args, cancel);
        }
    }
}