using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Flippo
{
    public class FlippoCopyArgs
    {
        public FlippoCopyArgs([NotNull] string fromReference, [NotNull] string toReference, bool? replace = null, IDictionary<string, object> values = null)
        {
            FromReference = fromReference ?? throw new ArgumentNullException(nameof(fromReference));
            ToReference = toReference ?? throw new ArgumentNullException(nameof(toReference));
            Values = values ?? new Dictionary<string, object>();
            Replace = replace;
        }

        [NotNull] public string FromReference { get; }
        [NotNull] public string ToReference { get; }
        public bool? Replace { get; }
        [NotNull] public IDictionary<string, object> Values { get; }
    }
}