using System;
using System.IO;
using System.Linq;
using google_photos_downloader.Properties;
using Microsoft.Win32;

namespace google_photos_downloader
{
    internal class PhotoSync
    {
        public PhotoSync()
        {
            client = new GooglePhotosClient();
        }

        private GooglePhotosClient client { get; }

        public DateTime execute(DateTime from)
        {
            Console.WriteLine("Fetching metadata...");
            var mediaObjects = client.getAllMediaObjects(from);
            var maxCreationDate = DateTime.MinValue;
            if (mediaObjects.Count > 0) maxCreationDate = mediaObjects.Max(media => media.mediaMetadata.creationTime);
            Console.WriteLine("Complete!");
            var path = Settings.Default.localRepositoryLocation;

            if (string.IsNullOrWhiteSpace(path))
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\GooglePhotoSync\";
                Console.WriteLine("localRepositoryLocation was not set in the settings, using {0} as the path...",
                    path);
            }

            if (!Directory.Exists(path)) throw new Exception(string.Format("Path {0} does not exist.", path));
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
            }

            var counter = 1;
            foreach (var mediaObject in mediaObjects)
            {
                var ext = GetDefaultExtension(mediaObject.mimeType);
                if (string.IsNullOrEmpty(ext)) ext = ".mp4";
                var filePath = string.Format("{0}{1}{2}", path, mediaObject.id, ext);
                try
                {
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("Image {0} of {1} doesn't exist, downloading...", counter++,
                            mediaObjects.Count);
                        client.downloadMediaFile(mediaObject.DownloadUrl, filePath);
                    }
                    else
                    {
                        Console.WriteLine("Image {0} of {1} already exists; skipping...", counter++,
                            mediaObjects.Count);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to download: {0} Continuing onwards...", e.Message);
                }
            }

            return maxCreationDate;
        }

        private string GetDefaultExtension(string mimeType)
        {
            string result;
            RegistryKey key;
            object value;

            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }
    }
}