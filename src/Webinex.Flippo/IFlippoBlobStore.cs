using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Webinex.Flippo
{
    public interface IFlippoBlobStore
    {
        Task<string> StoreAsync([NotNull] FlippoStoreBlobArgs args, CancellationToken cancel = default);

        Task DeleteAsync([NotNull] string reference, [NotNull] IDictionary<string, object> values,
            CancellationToken cancel = default);

        Task<FlippoContent> ContentAsync([NotNull] string reference, [NotNull] IDictionary<string, object> values,
            CancellationToken cancel = default);

        Task<bool> ExistsAsync([NotNull] string reference, [NotNull] IDictionary<string, object> values,
            CancellationToken cancel = default);

        Task CopyAsync([NotNull] string from, [NotNull] string to, [NotNull] IDictionary<string, object> values,
            CancellationToken cancel = default);

        Task MoveAsync([NotNull] string from, [NotNull] string to, [NotNull] IDictionary<string, object> values,
            CancellationToken cancel = default);
    }
}