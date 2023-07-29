using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Webinex.Coded;

namespace Webinex.Flippo
{
    public interface IFlippo
    {
        Task<CodedResult<string>> StoreAsync([NotNull] FlippoStoreArgs args, CancellationToken cancel = default);

        Task<CodedResult> DeleteAsync([NotNull] FlippoDeleteArgs args, CancellationToken cancel = default);

        Task<CodedResult<FlippoContent>> GetAsync([NotNull] FlippoGetArgs args, CancellationToken cancel = default);

        Task<CodedResult<bool>> ExistsAsync([NotNull] FlippoExistsArgs args, CancellationToken cancel = default);

        Task<CodedResult> CopyAsync([NotNull] FlippoCopyArgs args, CancellationToken cancel = default);

        Task<CodedResult> MoveAsync([NotNull] FlippoMoveArgs args, CancellationToken cancel = default);
        Task<CodedResult<string>> GetSasTokenAsync([NotNull] FlippoGetSasTokenArgs args);
        Task<CodedResult> VerifySasTokenAsync([NotNull] FlippoVerifySasTokenArgs args);
    }

    public static class FlippoExtensions
    {
        public static Task<CodedResult<string>> StoreAsync(
            [NotNull] this IFlippo flippo,
            [NotNull] Stream stream,
            string reference = null,
            string fileName = null,
            string mimeType = null,
            bool? replace = null,
            IDictionary<string, object> values = null,
            CancellationToken cancel = default)
        {
            flippo = flippo ?? throw new ArgumentNullException(nameof(flippo));
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            var args = new FlippoStoreArgs(stream, fileName, mimeType, reference, replace, values);
            return flippo.StoreAsync(args, cancel);
        }

        public static Task<CodedResult> DeleteAsync(
            [NotNull] this IFlippo flippo,
            [NotNull] string reference,
            IDictionary<string, object> values = null,
            CancellationToken cancel = default)
        {
            flippo = flippo ?? throw new ArgumentNullException(nameof(flippo));
            reference = reference ?? throw new ArgumentNullException(nameof(reference));

            return flippo.DeleteAsync(new FlippoDeleteArgs(reference, values), cancel);
        }

        public static Task<CodedResult<FlippoContent>> GetAsync(
            [NotNull] this IFlippo flippo,
            [NotNull] string reference,
            IDictionary<string, object> values = null,
            CancellationToken cancel = default)
        {
            flippo = flippo ?? throw new ArgumentNullException(nameof(flippo));
            reference = reference ?? throw new ArgumentNullException(nameof(reference));

            return flippo.GetAsync(new FlippoGetArgs(reference, values), cancel);
        }

        public static Task<CodedResult<bool>> ExistsAsync(
            [NotNull] this IFlippo flippo,
            [NotNull] string reference,
            IDictionary<string, object> values = null,
            CancellationToken cancel = default)
        {
            flippo = flippo ?? throw new ArgumentNullException(nameof(flippo));
            reference = reference ?? throw new ArgumentNullException(nameof(reference));
            return flippo.ExistsAsync(new FlippoExistsArgs(reference, values), cancel);
        }

        public static Task<CodedResult> MoveAsync(
            [NotNull] this IFlippo flippo,
            [NotNull] string fromReference,
            [NotNull] string toReference,
            bool? replace = null,
            IDictionary<string, object> values = null,
            CancellationToken cancel = default)
        {
            flippo = flippo ?? throw new ArgumentNullException(nameof(flippo));
            fromReference = fromReference ?? throw new ArgumentNullException(nameof(fromReference));
            toReference = toReference ?? throw new ArgumentNullException(nameof(toReference));

            var moveArgs = new FlippoMoveArgs(fromReference, toReference, values, replace);
            return flippo.MoveAsync(moveArgs, cancel);
        }

        public static Task<CodedResult> CopyAsync(
            [NotNull] this IFlippo flippo,
            [NotNull] string fromReference,
            [NotNull] string toReference,
            bool? replace = null,
            IDictionary<string, object> values = null,
            CancellationToken cancel = default)
        {
            flippo = flippo ?? throw new ArgumentNullException(nameof(flippo));
            fromReference = fromReference ?? throw new ArgumentNullException(nameof(fromReference));
            toReference = toReference ?? throw new ArgumentNullException(nameof(toReference));

            var moveArgs = new FlippoCopyArgs(fromReference, toReference, replace, values);
            return flippo.CopyAsync(moveArgs, cancel);
        }

        public static Task<CodedResult<string>> GetSasTokenAsync(this IFlippo flippo, [NotNull] IEnumerable<string> references)
        {
            return flippo.GetSasTokenAsync(new FlippoGetSasTokenArgs(references));
        }

        public static Task<CodedResult<string>> GetSasTokenAsync(this IFlippo flippo, [NotNull] string reference)
        {
            return flippo.GetSasTokenAsync(new FlippoGetSasTokenArgs(new[] { reference }));
        }

        public static Task<CodedResult> VerifySasTokenAsync(
            this IFlippo flippo,
            [NotNull] string token,
            [NotNull] IEnumerable<string> references)
        {
            return flippo.VerifySasTokenAsync(new FlippoVerifySasTokenArgs(token, references));
        }

        public static Task<CodedResult> VerifySasTokenAsync(
            this IFlippo flippo,
            [NotNull] string token,
            [NotNull] string reference)
        {
            return flippo.VerifySasTokenAsync(new FlippoVerifySasTokenArgs(token, new[] { reference }));
        }
    }
}