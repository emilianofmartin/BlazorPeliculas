using Azure.Storage.Blobs;

namespace BlazorPeliculas.Server.Helpers {
    public class FileSaverAzStorage : IFileSaver {
        private string connectionString;

        public FileSaverAzStorage(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("AzureStorage")!;
        }
        public async Task DeleteFile(string path, string containerName) {
            var client = new BlobContainerClient(connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(path);
            var blob = client.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName) {
            var client = new BlobContainerClient(connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            if(!extension.StartsWith(".")) extension = "." + extension;
            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(fileName);

            using (var ms = new MemoryStream(content)) {
                await blob.UploadAsync(ms);
            }

            return blob.Uri.ToString(); 
        }
    }
}
