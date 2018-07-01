using System;

namespace google_photos_downloader
{
    public class GooglePhotosMediaObject
    {
        public string id { get; set; }
        public string description { get; set; }
        public string productUrl { get; set; }
        public string baseUrl { get; set; }
        public string mimeType { get; set; }
        public MediaMetadata mediaMetadata { get; set; }
        public ContributorInfo contributorInfo { get; set; }

        public string DownloadUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(mimeType))
                    return baseUrl + "=w" + mediaMetadata.width + "-h" + mediaMetadata.height;
                return baseUrl + "=dv";
            }
        }

        public class Photo
        {
        }

        public class MediaMetadata
        {
            public string width { get; set; }
            public DateTime creationTime { get; set; }
            public Photo photo { get; set; }
            public string height { get; set; }
        }

        public class ContributorInfo
        {
            public string profilePictureBaseUrl { get; set; }
            public string displayName { get; set; }
        }
    }
}