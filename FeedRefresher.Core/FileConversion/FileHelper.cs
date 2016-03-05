using System.IO;

namespace FeedRefresher.Core.FileConversion
{
    public class FileHelper
    {
        public static bool IsFileM4a(string downloadedFilePath)
        {
            var extension = Path.GetExtension(downloadedFilePath);

            return extension.ToLower() == ".m4a";
        }
    }
}
