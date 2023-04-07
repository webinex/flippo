using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Flippo
{
    public class FlippoMoveArgs
    {
        public FlippoMoveArgs([NotNull] string fromReference, [NotNull] string toReference,
            IDictionary<string, object> values = null, bool? replace = null)
        {
            FromReference = fromReference ?? throw new ArgumentNullException(nameof(fromReference));
            ToReference = toReference ?? throw new ArgumentNullException(nameof(toReference));
            Replace = replace;
            Values = values ?? new Dictionary<string, object>();
        }

        [NotNull] public string FromReference { get; }

        [NotNull] public string ToReference { get; }
        
        public bool? Replace { get; }

        [NotNull] public IDictionary<string, object> Values { get; }
    }
}