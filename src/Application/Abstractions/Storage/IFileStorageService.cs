namespace Application.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string organizationId, CancellationToken cancellationToken);
    Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
}
