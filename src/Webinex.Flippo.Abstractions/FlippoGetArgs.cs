using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Flippo
{
    public class FlippoGetArgs
    {
        public FlippoGetArgs([NotNull] string reference, IDictionary<string, object> values = null)
        {
            Reference = reference ?? throw new ArgumentNullException(nameof(reference));
            Values = values ?? new Dictionary<string, object>();
        }

        [NotNull] public string Reference { get; }
        [NotNull] public IDictionary<string, object> Values { get; }
    }
}