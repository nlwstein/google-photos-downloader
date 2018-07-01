using System;
using google_photos_downloader.Properties;

namespace google_photos_downloader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(Settings.Default.user))
            {
                Console.Write("Please enter a valid Google username (eg: test@gmail.com): ");
                Settings.Default.user = Console.ReadLine();
                Settings.Default.Save();
            }

            var lastSync = Settings.Default.lastSyncTime;
            var photoSync = new PhotoSync();
            var result = photoSync.execute(lastSync);
            if (result != DateTime.MinValue)
            {
                Settings.Default.lastSyncTime = result;
                Settings.Default.Save();
            }
        }
    }
}