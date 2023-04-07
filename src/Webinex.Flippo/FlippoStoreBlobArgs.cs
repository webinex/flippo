using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Webinex.Flippo
{
    public class FlippoStoreBlobArgs
    {
        public FlippoStoreBlobArgs(FlippoStoreArgs args)
            : this(
                args.Stream,
                args.Reference,
                args.FileName,
                args.MimeType,
                args.Values)
        {
        }

        public FlippoStoreBlobArgs(
            Stream stream,
            string reference,
            string fileName,
            string mimeType,
            IDictionary<string, object> values)
        {
            Stream = stream;
            Reference = reference;
            FileName = fileName;
            MimeType = mimeType;
            Values = values;
        }

        [NotNull] public Stream Stream { get; }

        [MaybeNull] public string Reference { get; }

        [MaybeNull] public string FileName { get; }

        [MaybeNull] public string MimeType { get; }

        [NotNull] public IDictionary<string, object> Values { get; }
    }
}