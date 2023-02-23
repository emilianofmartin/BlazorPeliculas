namespace BlazorPeliculas.Server.Helpers {
    public class FileSaverLocal : IFileSaver {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FileSaverLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task DeleteFile(string path, string containerName) {
            var fileName = Path.GetFileName(path);
            var folderFileName = Path.Combine(env.WebRootPath, containerName, fileName);

            if(File.Exists(folderFileName))
                File.Delete(folderFileName);

            return Task.CompletedTask;
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName) {
            if (!extension.StartsWith(".")) extension = "." + extension;
            var fileName = $"{Guid.NewGuid()}{extension}";
            var folder = Path.Combine(env.WebRootPath, containerName);

            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            string savePath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(savePath, content);

            var actualURL = $"{httpContextAccessor!.HttpContext!.Request.Scheme}://{httpContextAccessor!.HttpContext.Request.Host}";
            //En DBPath tendremos directorio/archivo
            var DBPath = Path.Combine(actualURL, containerName, fileName);
            DBPath = DBPath.Replace("\\", "/");

            return DBPath;
        }
    }
}
