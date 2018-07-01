using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using google_photos_downloader.Properties;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using Newtonsoft.Json;

namespace google_photos_downloader
{
    internal class GooglePhotosClient
    {
        private GoogleConfig _config;
        private readonly GooglePhotosApi api;
        private bool authenticated;
        private string token;

        public GooglePhotosClient()
        {
            api = new GooglePhotosApi();
        }

        private GoogleConfig config
        {
            get
            {
                if (_config == null)
                    _config = JsonConvert.DeserializeObject<GoogleConfig>(File.ReadAllText("google.json"));
                return _config;
            }
        }

        public bool downloadMediaFile(string url, string path)
        {
            return api.downloadMediaFile(url, path);
        }

        public List<GooglePhotosMediaObject> getAllMediaObjects(DateTime? date = null,
            List<GooglePhotosMediaObject> mediaGalleryCollection = null, string paginationToken = null)
        {
            if (mediaGalleryCollection == null) mediaGalleryCollection = new List<GooglePhotosMediaObject>();
            if (!authenticated) authenticate();
            var rawResponse = api.query(GooglePhotosApi.LIBRARY_SEARCH_ENDPOINT, new MediaGallerySearchRequest
            {
                pageSize = "500",
                pageToken = paginationToken
            });
            var response = JsonConvert.DeserializeObject<MediaGallerySearchResponse>(rawResponse);
            var onlySetRequired = false; //  
            if (date.HasValue)
            {
                var totalItems = response.mediaItems.Count;
                response.mediaItems = response.mediaItems.Where(media => media.mediaMetadata.creationTime > date.Value)
                    .ToList();
                if (response.mediaItems.Count != totalItems) onlySetRequired = true;
            }

            if (response.mediaItems != null) mediaGalleryCollection.AddRange(response.mediaItems);
            if (!string.IsNullOrEmpty(response.nextPageToken) && !onlySetRequired)
                return getAllMediaObjects(date, mediaGalleryCollection, response.nextPageToken);
            return mediaGalleryCollection;
        }

        private void authenticate()
        {
            var authorizationTask = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = config.installed.client_id,
                    ClientSecret = config.installed.client_secret
                },
                new[] {"https://www.googleapis.com/auth/photoslibrary"},
                Settings.Default.user,
                CancellationToken.None,
                null,
                new LocalServerCodeReceiver()
            );
            authorizationTask.Wait();
            var authorizationResult = authorizationTask.Result;
            if (authorizationResult.Token.IsExpired(SystemClock.Default))
            {
                var refreshTask = authorizationResult.RefreshTokenAsync(new CancellationToken());
                refreshTask.Wait();
            }

            api.token = authorizationResult.Token.AccessToken;
            authenticated = true;
        }
    }
}