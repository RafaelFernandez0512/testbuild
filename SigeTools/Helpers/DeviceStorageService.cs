using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
namespace SigeTools.Helpers
{
    
    public class DeviceStorageService : IDeviceStorageService
    {
        
        private readonly string _environment = FileSystem.CacheDirectory;

        public async Task<string> Write(MemoryStream ms, string fileName)
        {
            try
            {
                var file = Path.Combine(_environment, $"{fileName?.Trim()}.pdf");
                 using var writer = new FileStream(file, FileMode.Create, System.IO.FileAccess.ReadWrite);
                var bytes = ms.ToArray();
                writer.Write(bytes,0,bytes.Length);
                await writer.WriteAsync(bytes, 0, bytes.Length);
                if (writer.CanRead)
                {
                    return file;
                }
                
                throw new Exception("Denied Permission storage");
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        
        public async Task<string> Write(MemoryStream ms, string fileName,string extension)
        {
            try
            {
                var file = Path.Combine(_environment, $"{fileName?.Trim()}.{extension}");
                 using var writer = new FileStream(file, FileMode.Create, System.IO.FileAccess.ReadWrite);
                var bytes = ms.ToArray();
                writer.Write(bytes,0,bytes.Length);
                await writer.WriteAsync(bytes, 0, bytes.Length);
                if (writer.CanRead)
                {
                    return file;
                }
                
                throw new Exception("Denied Permission storage");
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public async Task<Stream> GetFile(string fileName)
        {
            var file = Path.Combine(_environment, $"{fileName}.pdf");
            try
            {
                MemoryStream ms = new MemoryStream();
                 using FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                await fileStream.CopyToAsync(ms);
                return ms;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public async Task<byte[]> GetFileOnByteArray(string path)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                 using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                await fileStream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public async Task<Stream> GetFileByPath(string path)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                var filePath = path;
                 using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                await fileStream.CopyToAsync(ms);
                return ms;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public async Task<Stream> CreateZipFromFiles(List<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
            {
                throw new ArgumentException("La lista de rutas de archivos está vacía o es nula.");
            }

            var memoryStream = new MemoryStream();

            try
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (string filePath in filePaths)
                    {
                        if (File.Exists(filePath))
                        {
                            string entryName = Path.GetFileName(filePath);
                            ZipArchiveEntry entry = archive.CreateEntry(entryName);

                            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            using (Stream entryStream = entry.Open())
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                        else
                        {
                            throw new FileNotFoundException($"El archivo no se encontró en la ruta: {filePath}");
                        }
                    }
                }

                memoryStream.Position = 0; // Asegurarse de que el Stream esté en la posición correcta.
                return memoryStream;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
    }
}