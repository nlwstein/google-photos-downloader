using System;
using System.Collections.Generic;

namespace google_photos_downloader
{
    public class MediaObjectsResult
    {
        public List<GooglePhotosMediaObject> mediaObjects { get; set; }
        public DateTime maxCreationDate { get; set; }
    }
}