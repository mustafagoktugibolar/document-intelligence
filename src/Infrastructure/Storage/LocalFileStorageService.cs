using Application.Abstractions.Storage;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Storage;

internal sealed class LocalFileStorageService(IConfiguration configuration) : IFileStorageService
{
    private readonly string _basePath = configuration["Storage:BasePath"]
        ?? Path.Combine(Path.GetTempPath(), "doc-intelligence");

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string organizationId,
        CancellationToken cancellationToken)
    {
        string safeFileName = Path.GetFileName(fileName);
        string storageKey = $"{organizationId}/{Guid.NewGuid()}/{safeFileName}";

        string fullPath = ToFullPath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using FileStream fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return storageKey;
    }

    public Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken)
    {
        Stream stream = File.OpenRead(ToFullPath(storageKey));
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        string fullPath = ToFullPath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }

    private string ToFullPath(string storageKey) =>
        Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));
}
