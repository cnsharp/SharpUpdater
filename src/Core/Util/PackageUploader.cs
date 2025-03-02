using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace CnSharp.Updater.Util
{
    public class PackageUploader
    {
        public static async Task Upload(string packageFile, string source, string apiKey)
        {
            if (!packageFile.EndsWith(Constants.PackageExtension))
                throw new InvalidDataException("Invalid package format.");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add(Constants.ApiKeyHeader, apiKey);

                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(await ReadAllBytesAsync(packageFile));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    content.Add(fileContent, "file", Path.GetFileName(packageFile));

                    var response = await httpClient.PutAsync(source, content);
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public static async Task<byte[]> ReadAllBytesAsync(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
