namespace BlazorPeliculasServer.Helpers {
    public interface IFileSaver {
        Task<string> SaveFile(byte[] content, string extension, string containerName);
        Task DeleteFile(string path, string containerName);
        async Task<string> EditFile(byte[] content, string extension, string containerName, string path) {
            if(path is not null) {
                await DeleteFile(path, containerName);
            }

            return await SaveFile(content, extension, containerName);
        }
    }
}
