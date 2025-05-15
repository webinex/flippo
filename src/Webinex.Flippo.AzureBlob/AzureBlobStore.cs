using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Webinex.Flippo.AzureBlob
{
    internal class AzureBlobStore : IFlippoBlobStore
    {
        private readonly TimeSpan SAS_TIMESPAN = TimeSpan.FromSeconds(30);
        private readonly Lazy<BlobContainerClient> _clientLazy;

        public AzureBlobStore(AzureBlobSettings settings)
        {
            _clientLazy = new Lazy<BlobContainerClient>(() =>
                new BlobContainerClient(settings.ConnectionString, settings.Container));
        }

        private BlobContainerClient Client => _clientLazy.Value;

        public async Task<string> StoreAsync(FlippoStoreBlobArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            var reference = Reference(args.Reference);
            var metadata = Metadata.From(args);
            var blob = Client.GetBlobClient(reference);
            await blob.UploadAsync(args.Stream, metadata: metadata.ToDictionary(), cancellationToken: cancel);

            return reference;
        }

        public async Task DeleteAsync(string reference, IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            reference = reference ?? throw new ArgumentNullException(nameof(reference));
            await Client.GetBlobClient(reference).DeleteAsync(cancellationToken: cancel);
        }

        public async Task<FlippoContent> ContentAsync(string reference, IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            reference = reference ?? throw new ArgumentNullException(nameof(reference));

            Response<BlobDownloadResult> blob = null;
            try
            {
                blob = await Client.GetBlobClient(reference).DownloadContentAsync(cancel);
            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == BlobErrorCode.BlobNotFound)
                    return null;

                throw;
            }
            
            var metadata = Metadata.From(blob.Value.Details.Metadata);

            Stream stream = null;
            try
            {
                stream = blob.Value.Content.ToStream();
                return new FlippoContent(reference, stream, metadata.FileName, metadata.MimeType);
            }
            catch (Exception)
            {
                if (stream != null)
                {
                    await stream.DisposeAsync();
                }

                throw;
            }
        }

        public async Task<bool> ExistsAsync(string reference, IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            reference = reference ?? throw new ArgumentNullException(nameof(reference));
            var result = await Client.GetBlobClient(reference).ExistsAsync(cancel);
            return result.Value;
        }

        public async Task CopyAsync(string from, string to, IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            from = from ?? throw new ArgumentNullException(nameof(from));
            to = to ?? throw new ArgumentNullException(nameof(to));

            var fromBlob = Client.GetBlobClient(from);
            var toBlob = Client.GetBlobClient(to);
            var fromBlobUri = fromBlob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(SAS_TIMESPAN));

            var properties = await fromBlob.GetPropertiesAsync(cancellationToken: cancel);
            var copyOptions = new BlobCopyFromUriOptions { Metadata = properties.Value.Metadata };
            await toBlob.SyncCopyFromUriAsync(fromBlobUri, copyOptions, cancel);
        }

        public async Task MoveAsync(string from, string to, IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            from = from ?? throw new ArgumentNullException(nameof(from));
            to = to ?? throw new ArgumentNullException(nameof(to));

            await CopyAsync(from, to, values, cancel);
            await Client.GetBlobClient(from).DeleteAsync(cancellationToken: cancel);
        }

        private string Reference(string reference = null)
        {
            return reference ?? Guid.NewGuid().ToString("N");
        }

        private class Metadata
        {
            public string FileName { get; set; }
            public string MimeType { get; set; }

            public IDictionary<string, string> ToDictionary()
            {
                return new Dictionary<string, string>
                {
                    [nameof(FileName)] = EscapeHeader(FileName),
                    [nameof(MimeType)] = EscapeHeader(MimeType),
                };
            }

            public static Metadata From(IDictionary<string, string> metadata)
            {
                if (metadata == null)
                    return new Metadata();

                metadata.TryGetValue(nameof(FileName), out var fileName);
                metadata.TryGetValue(nameof(MimeType), out var mimeType);

                return new Metadata { FileName = UnescapeHeader(fileName), MimeType = UnescapeHeader(mimeType) };
            }

            public static Metadata From(FlippoStoreBlobArgs args)
            {
                return new Metadata { FileName = args.FileName, MimeType = args.MimeType };
            }

            private static string EscapeHeader(string value)
            {
                return Uri.EscapeDataString(value);
            }

            private static string UnescapeHeader(string value)
            {
                return Uri.UnescapeDataString(value);
            }
        }
    }
}