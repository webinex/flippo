﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Flippo
{
    public class FileSystemBlobSettings
    {
        public FileSystemBlobSettings([NotNull] string basePath)
        {
            BasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        public FileSystemBlobSettings()
        {
        }

        public string BasePath { get; set; }
    }
}