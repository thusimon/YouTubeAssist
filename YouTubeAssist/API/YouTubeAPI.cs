using System.Configuration;
using System.Diagnostics;
using Google;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeAssist.API
{
    internal class YouTubeAPI
    {
        YouTubeService service;
        public YouTubeAPI() {
            service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["ApiKey"],
                ApplicationName = this.GetType().ToString()
            });
        }

        public async Task<Tuple<List<string>, List<string>, List<string>>> Search(string keyword)
        {
            SearchResource.ListRequest searchListRequest = service.Search.List("snippet");
            searchListRequest.Q = keyword;
            searchListRequest.MaxResults = 50;

            SearchListResponse searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (SearchResult searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            Tuple<List<string>, List<string>, List<string>> result = Tuple.Create(videos, channels, playlists);

            Debug.WriteLine($"Videos:\n{string.Join("\n", videos)}\n");
            Debug.WriteLine($"Channels:\n{string.Join("\n", channels)}\n");
            Debug.WriteLine($"Playlists:\n{string.Join("\n", playlists)}\n");

            return result;
        }

        public async Task<Channel?> SearchChannel(string handleName)
        {
            ChannelsResource.ListRequest channelListRequest = service.Channels.List("snippet,contentDetails,statistics");
            channelListRequest.ForHandle = handleName;
            channelListRequest.MaxResults = 1;

            ChannelListResponse? channelListResponse = null;
            try
            {
                channelListResponse = await channelListRequest.ExecuteAsync();
            }
            catch (GoogleApiException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
            

            if (channelListResponse == null || channelListResponse.Items == null)
            {
                return null;
            }

            Google.Apis.YouTube.v3.Data.Channel channelResult = channelListResponse.Items[0];
            if (channelResult == null)
            {
                return null;
            }

            Uri? customeUrl = null;
            try
            {
                string customeUrlResult = channelResult.Snippet.CustomUrl;
                if (!customeUrlResult.StartsWith("http"))
                {
                    customeUrl = new Uri("https://www.youtube.com/" + customeUrlResult);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to convert customeUrl to Uri", ex);
            }

            Uri? thumbUrl = null;
            try
            {
                thumbUrl = new Uri(channelResult.Snippet.Thumbnails.Default__.Url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to convert thumbUrl to Uri", ex);
            }

            Channel channel = new Channel(channelResult.Id,
                channelResult.Snippet.Title,
                channelResult.Snippet.Description,
                customeUrl,
                thumbUrl,
                channelResult.Statistics.ViewCount ?? 0,
                channelResult.Statistics.VideoCount ?? 0);
            channel.Date = channelResult.Snippet.PublishedAtDateTimeOffset;
            return channel;
        }

    }
}
