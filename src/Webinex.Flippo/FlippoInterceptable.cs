using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Webinex.Coded;

namespace Webinex.Flippo
{
    internal interface IFlippoInterceptable : IFlippo
    {
    }

    internal class FlippoInterceptable : IFlippoInterceptable
    {
        private readonly IFlippoBlobStore _blob;
        private readonly IFlippoSasTokenService _sasTokenService;

        public FlippoInterceptable(IFlippoBlobStore blob, IFlippoSasTokenService sasTokenService)
        {
            _blob = blob;
            _sasTokenService = sasTokenService;
        }

        public async Task<CodedResult<string>> StoreAsync(FlippoStoreArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            cancel.ThrowIfCancellationRequested();
            var destinationResult = await ValidateDestinationAsync(args.Reference, args.Replace, args.Values, cancel);
            if (!destinationResult.Succeed) return new CodedResult<string>(destinationResult.Failure, null);

            cancel.ThrowIfCancellationRequested();
            var storeArgs = new FlippoStoreBlobArgs(args);
            var reference = await _blob.StoreAsync(storeArgs, cancel);
            return CodedResults.Success(reference);
        }

        public async Task<CodedResult> DeleteAsync(FlippoDeleteArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            var existsResult = await ValidateExistsAsync(args.Reference, args.Values, cancel);
            if (!existsResult.Succeed) return existsResult;

            cancel.ThrowIfCancellationRequested();
            await _blob.DeleteAsync(args.Reference, args.Values, cancel);
            return CodedResults.Success();
        }

        public async Task<CodedResult<FlippoContent>> GetAsync(FlippoGetArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            var content = await _blob.ContentAsync(args.Reference, args.Values, cancel);
            return content == null
                ? FlippoFailures.NotFound(args.Reference).ToResult<FlippoContent>()
                : CodedResults.Success(content);
        }

        public async Task<CodedResult<bool>> ExistsAsync(FlippoExistsArgs args, CancellationToken cancel = default)
        {
            cancel.ThrowIfCancellationRequested();

            var exists = await _blob.ExistsAsync(args.Reference, args.Values, cancel);
            return CodedResults.Success(exists);
        }

        public async Task<CodedResult> CopyAsync(FlippoCopyArgs args, CancellationToken cancel = default)
        {
            cancel.ThrowIfCancellationRequested();
            var fromExistsResult = await ValidateExistsAsync(args.FromReference, args.Values, cancel);
            if (!fromExistsResult.Succeed) return fromExistsResult;

            cancel.ThrowIfCancellationRequested();
            var destinationResult = await ValidateDestinationAsync(args.ToReference, args.Replace, args.Values, cancel);
            if (!destinationResult.Succeed) return destinationResult;

            cancel.ThrowIfCancellationRequested();
            await _blob.CopyAsync(args.FromReference, args.ToReference, args.Values, cancel);
            return CodedResults.Success();
        }

        private async Task<CodedResult> ValidateExistsAsync(string reference, IDictionary<string, object> values,
            CancellationToken cancel)
        {
            return !await _blob.ExistsAsync(reference, values, cancel)
                ? FlippoFailures.NotFound(reference).ToResult()
                : CodedResults.Success();
        }

        private async Task<CodedResult> ValidateDestinationAsync(
            string destination,
            bool? replace,
            IDictionary<string, object> values,
            CancellationToken cancel)
        {
            return replace == true || destination == null || !await _blob.ExistsAsync(destination, values, cancel)
                ? CodedResults.Success()
                : FlippoFailures.Exists(destination).ToResult();
        }

        public async Task<CodedResult> MoveAsync(FlippoMoveArgs args, CancellationToken cancel = default)
        {
            cancel.ThrowIfCancellationRequested();
            var fromExistsResult = await ValidateExistsAsync(args.FromReference, args.Values, cancel);
            if (!fromExistsResult.Succeed) return fromExistsResult;

            cancel.ThrowIfCancellationRequested();
            var destinationResult = await ValidateDestinationAsync(args.ToReference, args.Replace, args.Values, cancel);
            if (!destinationResult.Succeed) return destinationResult;

            cancel.ThrowIfCancellationRequested();
            await _blob.MoveAsync(args.FromReference, args.ToReference, args.Values, cancel);
            return CodedResults.Success();
        }

        public Task<CodedResult<string>> GetSasTokenAsync(FlippoGetSasTokenArgs args)
        {
            return _sasTokenService.GetSasTokenAsync(args.References);
        }

        public Task<CodedResult> VerifySasTokenAsync(FlippoVerifySasTokenArgs args)
        {
            return _sasTokenService.VerifySasTokenAsync(args.Token, args.References);
        }
    }
}