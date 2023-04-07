using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Webinex.Coded;

namespace Webinex.Flippo.Interceptors
{
    /// <summary>
    ///     Forward control to next pipeline member
    /// </summary>
    /// <typeparam name="TArgs">Argument type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public interface INext<TArgs, TResult>
    {
        /// <summary>
        ///     Forward control to next pipeline member
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>Next pipeline members result</returns>
        Task<TResult> InvokeAsync(TArgs args);
    }

    /// <summary>
    ///     Interceptors behavior similar to middlewares. They would be executed
    ///     in reverse order they added and same order after calling `next.InvokeAsync(args)`.
    ///     If interceptor returns failed CodedResult, than request pipeline interrupted and CodedResult returned.  
    ///     Default ClippoController would return failed status code with serialized `x-coded-failure` header.
    ///
    ///     If you would like to intercept only part of methods, you can use <see cref="FlippoInterceptor"/>
    /// </summary>
    public interface IFlippoInterceptor
    {
        /// <summary>
        ///     Interceptor for <see cref="IFlippo.StoreAsync"/>
        /// </summary>
        Task<CodedResult<string>> OnStoreAsync(
            [NotNull] FlippoStoreArgs args,
            [NotNull] INext<FlippoStoreArgs, CodedResult<string>> next);

        
        /// <summary>
        ///     Interceptor for <see cref="IFlippo.DeleteAsync"/>
        /// </summary>
        Task<CodedResult> OnDeleteAsync(
            [NotNull] FlippoDeleteArgs args,
            [NotNull] INext<FlippoDeleteArgs, CodedResult> next);

        /// <summary>
        ///     Interceptor for <see cref="IFlippo.ExistsAsync"/>
        /// </summary>
        Task<CodedResult<bool>> OnExistsAsync(
            [NotNull] FlippoExistsArgs args,
            [NotNull] INext<FlippoExistsArgs, CodedResult<bool>> next);

        /// <summary>
        ///     Interceptor for <see cref="IFlippo.GetAsync"/>
        /// </summary>
        Task<CodedResult<FlippoContent>> OnGetAsync(
            [NotNull] FlippoGetArgs args,
            [NotNull] INext<FlippoGetArgs, CodedResult<FlippoContent>> next);

        /// <summary>
        ///     Interceptor for <see cref="IFlippo.CopyAsync"/>
        /// </summary>
        Task<CodedResult> OnCopyAsync(
            [NotNull] FlippoCopyArgs args,
            [NotNull] INext<FlippoCopyArgs, CodedResult> next);

        /// <summary>
        ///     Interceptor for <see cref="IFlippo.MoveAsync"/>
        /// </summary>
        Task<CodedResult> OnMoveAsync(
            [NotNull] FlippoMoveArgs args,
            [NotNull] INext<FlippoMoveArgs, CodedResult> next);
    }

    public class FlippoInterceptor : IFlippoInterceptor
    {
        public virtual Task<CodedResult<string>> OnStoreAsync(FlippoStoreArgs args, INext<FlippoStoreArgs, CodedResult<string>> next)
        {
            args = args ?? throw new ArgumentNullException();
            return next.InvokeAsync(args);
        }

        public virtual Task<CodedResult> OnDeleteAsync(FlippoDeleteArgs args, INext<FlippoDeleteArgs, CodedResult> next)
        {
            args = args ?? throw new ArgumentNullException();
            return next.InvokeAsync(args);
        }

        public virtual Task<CodedResult<bool>> OnExistsAsync(FlippoExistsArgs args, INext<FlippoExistsArgs, CodedResult<bool>> next)
        {
            args = args ?? throw new ArgumentNullException();
            return next.InvokeAsync(args);
        }

        public virtual Task<CodedResult<FlippoContent>> OnGetAsync(FlippoGetArgs args, INext<FlippoGetArgs, CodedResult<FlippoContent>> next)
        {
            args = args ?? throw new ArgumentNullException();
            return next.InvokeAsync(args);
        }

        public Task<CodedResult> OnCopyAsync(FlippoCopyArgs args, INext<FlippoCopyArgs, CodedResult> next)
        {
            args = args ?? throw new ArgumentNullException();
            return next.InvokeAsync(args);
        }

        public Task<CodedResult> OnMoveAsync(FlippoMoveArgs args, INext<FlippoMoveArgs, CodedResult> next)
        {
            args = args ?? throw new ArgumentNullException();
            return next.InvokeAsync(args);
        }
    }
}