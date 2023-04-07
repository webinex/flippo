using System;
using System.Threading;
using System.Threading.Tasks;
using Webinex.Coded;

namespace Webinex.Flippo.Interceptors
{
    
    internal class FlippoInterceptorActivator<TInterceptor> : IFlippoInterceptable
        where TInterceptor : IFlippoInterceptor
    {
        private readonly IFlippoInterceptable _flippo;
        private readonly TInterceptor _interceptor;

        public FlippoInterceptorActivator(IFlippoInterceptable flippo, TInterceptor interceptor)
        {
            _flippo = flippo;
            _interceptor = interceptor;
        }

        public Task<CodedResult<string>> StoreAsync(FlippoStoreArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            var next = new NextDelegate<FlippoStoreArgs, CodedResult<string>>(x => _flippo.StoreAsync(x, cancel));
            return _interceptor.OnStoreAsync(args, next);
        }

        public Task<CodedResult> DeleteAsync(FlippoDeleteArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            var next = new NextDelegate<FlippoDeleteArgs, CodedResult>(x => _flippo.DeleteAsync(x, cancel));
            return _interceptor.OnDeleteAsync(args, next);
        }

        public Task<CodedResult<FlippoContent>> GetAsync(FlippoGetArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            var next = new NextDelegate<FlippoGetArgs, CodedResult<FlippoContent>>(x => _flippo.GetAsync(x, cancel));
            return _interceptor.OnGetAsync(args, next);
        }

        public Task<CodedResult<bool>> ExistsAsync(FlippoExistsArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            var next = new NextDelegate<FlippoExistsArgs, CodedResult<bool>>(x => _flippo.ExistsAsync(x, cancel));
            return _interceptor.OnExistsAsync(args, next);
        }

        public Task<CodedResult> CopyAsync(FlippoCopyArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            var next = new NextDelegate<FlippoCopyArgs, CodedResult>(x => _flippo.CopyAsync(x, cancel));
            return _interceptor.OnCopyAsync(args, next);
        }

        public Task<CodedResult> MoveAsync(FlippoMoveArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            var next = new NextDelegate<FlippoMoveArgs, CodedResult>(x => _flippo.MoveAsync(x, cancel));
            return _interceptor.OnMoveAsync(args, next);
        }

        private class NextDelegate<TArgs, TResult> : INext<TArgs, TResult>
        {
            private readonly Func<TArgs, Task<TResult>> _delegate;

            public NextDelegate(Func<TArgs, Task<TResult>> @delegate)
            {
                _delegate = @delegate;
            }

            public Task<TResult> InvokeAsync(TArgs args)
            {
                return _delegate.Invoke(args);
            }
        }
    }
}