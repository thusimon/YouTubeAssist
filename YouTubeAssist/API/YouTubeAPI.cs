using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json.Serialization;

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

            Trace.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            Trace.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            Trace.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));

            return result;
        }

        public async Task<Channel?> SearchChannel(string handleName)
        {
            ChannelsResource.ListRequest channelListRequest = service.Channels.List("snippet,contentDetails,statistics");
            channelListRequest.ForHandle = handleName;
            channelListRequest.MaxResults = 1;

            ChannelListResponse channelListResponse = await channelListRequest.ExecuteAsync();

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
