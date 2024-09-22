using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeAssist.API
{
    //channelResult.Id,
    //            channelResult.Snippet.Title,
    //            channelResult.Snippet.Description,
    //            channelResult.Snippet.CustomUrl,
    //            channelResult.Snippet.Thumbnails.Default__.Url.ToLower(),
    //channelResult.Statistics.ViewCount??0,
    //            channelResult.Statistics.VideoCount??0
    public class Channel
    {
        private string? _id;
        private string? _title;
        private string? _description;
        private string? _customeUrl;
        private string? _thumbUrl;
        private ulong _viewCount;
        private ulong _videoCount;

        public string ID {
            get { return _id ?? "--"; }
            set { _id = value; } 
        }
        public string Title {
            get { return _title ?? "--"; }
            set { _title = value; }
        }
        public string Description {
            get { return _description ?? "--"; }
            set { _description = value; }
        }
        public string CustomUrl {
            get { return _customeUrl ?? "--"; }
            set { _customeUrl = value; }
        }
        public string ThumbUrl {
            get { return _thumbUrl ?? "--"; }
            set { _thumbUrl = value; }
        }
        public ulong ViewCount {
            get { return _viewCount; }
            set { _viewCount = value; }
        }
        public ulong VideoCount {
            get { return _videoCount; }
            set { _videoCount = value; }
        }

        public Channel(string? id = null, string? title = null, string? description = null, string? customeUrl = null, string? thumbUrl = null, ulong viewCount = 0, ulong videoCount = 0)
        {
            _id = id;
            _title = title;
            _description = description;
            _customeUrl = customeUrl;
            _thumbUrl = thumbUrl;
            _viewCount = viewCount;
            _videoCount = videoCount;
        }
    }
}
