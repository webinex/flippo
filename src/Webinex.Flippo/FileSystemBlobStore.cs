using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Webinex.Flippo
{
    internal class FileSystemBlobStore : IFlippoBlobStore
    {
        private readonly IOptions<FileSystemBlobSettings> _settings;

        public FileSystemBlobStore(IOptions<FileSystemBlobSettings> settings)
        {
            _settings = settings;
        }

        public async Task<string> StoreAsync(FlippoStoreBlobArgs args, CancellationToken cancel = default)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            cancel.ThrowIfCancellationRequested();

            var fileName = FileName(args);
            var manifestJson = Manifest.From(args).ToJson();
            var manifestFileName = ManifestFileName(fileName);

            await using var fileStream = File.Open(FilePath(fileName), FileMode.OpenOrCreate, FileAccess.Write);
            
            await Task.WhenAll(
                args.Stream.CopyToAsync(fileStream, cancel),
                File.WriteAllTextAsync(FilePath(manifestFileName), manifestJson, Encoding.UTF8, cancel));

            return fileName;
        }

        public Task DeleteAsync(string reference, IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            reference = reference ?? throw new ArgumentNullException(nameof(reference));
            cancel.ThrowIfCancellationRequested();
            
            File.Delete(FilePath(reference));
            return Task.CompletedTask;
        }

        public async Task<FlippoContent> ContentAsync(
            string reference,
            IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            reference = reference ?? throw new ArgumentNullException(nameof(reference));
            cancel.ThrowIfCancellationRequested();

            var filePath = FilePath(reference);

            if (!File.Exists(filePath))
                return null;

            var manifestPath = FilePath(ManifestFileName(reference));
            var manifestJson = await File.ReadAllTextAsync(manifestPath, Encoding.UTF8, cancel);
            var manifest = JsonSerializer.Deserialize<Manifest>(manifestJson)
                ?? throw new InvalidOperationException();

            var stream = File.OpenRead(filePath);
            return new FlippoContent(reference, stream, manifest.FileName, manifest.MimeType);
        }

        public Task<bool> ExistsAsync(string reference, IDictionary<string, object> values,
            CancellationToken cancel = default)
        {
            cancel.ThrowIfCancellationRequested();
            
            reference = reference ?? throw new ArgumentNullException(nameof(reference));
            var exists = File.Exists(FilePath(reference));
            return Task.FromResult(exists);
        }

        public Task CopyAsync(string from, string to, IDictionary<string, object> values, CancellationToken cancel = default)
        {
            from = from ?? throw new ArgumentNullException(nameof(from));
            to = to ?? throw new ArgumentNullException(nameof(to));
            
            cancel.ThrowIfCancellationRequested();
            File.Move(FilePath(from), FilePath(to));
            File.Move(FilePath(ManifestFileName(from)), FilePath(ManifestFileName(to)));
            
            return Task.CompletedTask;
        }

        public Task MoveAsync(string from, string to, IDictionary<string, object> values, CancellationToken cancel = default)
        {
            from = from ?? throw new ArgumentNullException(nameof(from));
            to = to ?? throw new ArgumentNullException(nameof(to));
            
            cancel.ThrowIfCancellationRequested();
            File.Copy(FilePath(from), FilePath(to), true);
            File.Copy(FilePath(ManifestFileName(from)), FilePath(ManifestFileName(to)), true);
            
            return Task.CompletedTask;
        }

        private string FileName(FlippoStoreBlobArgs args)
        {
            return args.Reference ?? Guid.NewGuid().ToString("N");
        }

        private string FilePath(string fileName)
        {
            fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            return Path.Combine(_settings.Value.BasePath, fileName);
        }

        private string ManifestFileName(string fileName)
        {
            fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            return $"{fileName}.manifest.json";
        }

        private class Manifest
        {
            public string FileName { get; set; }
            public string MimeType { get; set; }

            public static Manifest From(FlippoStoreBlobArgs args)
            {
                return new Manifest { FileName = args.FileName, MimeType = args.MimeType };
            }

            public string ToJson()
            {
                return JsonSerializer.Serialize(this);
            }
        }
    }
}