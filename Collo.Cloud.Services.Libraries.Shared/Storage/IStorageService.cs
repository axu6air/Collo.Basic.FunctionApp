namespace Collo.Cloud.Services.Libraries.Shared.Storage
{
    public interface IStorageService
    {
        Task DownloadBlobIfExistsAsync(Stream stream, string blobName);
        Task UploadBlobAsync(Stream stream, string blobName);
        Task DeleteBlobIfExistsAsync(string blobName);
        Task<bool> DoesBlobExistAsync(string blobName);
        Task<string> GetBlobUrl(string blobName);
    }
}
