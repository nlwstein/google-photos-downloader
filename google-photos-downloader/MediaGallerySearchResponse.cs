using System.Collections.Generic;

namespace google_photos_downloader
{
    public class MediaGallerySearchResponse
    {
        public List<GooglePhotosMediaObject> mediaItems { get; set; }
        public string nextPageToken { get; set; }
    }
}