using System.Collections.Generic;
using System.IO;
using RestSharp;

namespace google_photos_downloader
{
    internal class GooglePhotosApi
    {
        public const string BASE_URL = "https://photoslibrary.googleapis.com/";
        public const string LIBRARY_SEARCH_ENDPOINT = "/v1/mediaItems:search";

        protected RestClient http;

        public GooglePhotosApi()
        {
            http = new RestClient(BASE_URL);
        }

        public IDictionary<string, string> headers => new Dictionary<string, string>
        {
            ["Content-Type"] = "application/json",
            ["Authorization"] = "Bearer " + token
        };

        public string token { get; set; }

        public string query(string uri, object requestBody, Method httpMethod = Method.POST)
        {
            var request = new RestRequest(uri, httpMethod);
            request.AddJsonBody(requestBody);
            foreach (var header in headers) request.AddHeader(header.Key, header.Value);
            var response = http.Execute(request);
            return response.Content;
        }

        public bool downloadMediaFile(string url, string path)
        {
            var media = http.DownloadData(new RestRequest(url));
            File.WriteAllBytes(path, media);
            return true;
        }
    }
}