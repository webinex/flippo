using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Flippo
{
    internal class FileSystemBlobSettings
    {
        public FileSystemBlobSettings([NotNull] string basePath)
        {
            BasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        public string BasePath { get; }
    }
}