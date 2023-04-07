using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Webinex.Flippo
{
    public class FlippoContent : IDisposable
    {
        public FlippoContent([NotNull] string reference, [NotNull] Stream stream, string fileName = null, string mimeType = null)
        {
            Reference = reference ?? throw new ArgumentNullException(nameof(reference));
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            FileName = fileName;
            MimeType = mimeType;
        }

        [NotNull]
        public string Reference { get; }
        
        [NotNull]
        public Stream Stream { get; }
        
        [MaybeNull]
        public string FileName { get; }
        
        [MaybeNull]
        public string MimeType { get; }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}