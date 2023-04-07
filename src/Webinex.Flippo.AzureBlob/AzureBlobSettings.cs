using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Flippo.AzureBlob
{
    public class AzureBlobSettings
    {
        public AzureBlobSettings([NotNull] string connectionString, [NotNull] string container)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        [NotNull] public string ConnectionString { get; }
        [NotNull] public string Container { get; set; }
    }
}