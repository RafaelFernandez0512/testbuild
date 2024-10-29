namespace SigeTools.MAUI.Helpers
{
    public interface IDeviceStorageService
    {
        Task<string> Write(MemoryStream file,string fileName);
         Task<string> Write(MemoryStream ms, string fileName, string extension);

          Task<Stream> GetFile(string fileName);
         Task<byte[]> GetFileOnByteArray(string path);
         Task<Stream> GetFileByPath(string path);
        Task<Stream> CreateZipFromFiles(List<string> filePaths);
    }
}