using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Webinex.Flippo
{
    public class FlippoStoreArgs
    {
        public FlippoStoreArgs(
            [NotNull] Stream stream,
            string fileName = null,
            string mimeType = null,
            string reference = null,
            bool? replace = null,
            IDictionary<string, object> values = null)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            FileName = fileName;
            MimeType = mimeType;
            Reference = reference;
            Replace = replace;
            Values = values ?? new Dictionary<string, object>();
        }

        [NotNull] public Stream Stream { get; }

        [MaybeNull] public string Reference { get; }

        [MaybeNull] public string FileName { get; }

        [MaybeNull] public string MimeType { get; }

        public bool? Replace { get; }

        [NotNull] public IDictionary<string, object> Values { get; }
    }
}