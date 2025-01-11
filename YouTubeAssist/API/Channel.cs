namespace YouTubeAssist.API
{
    public class Channel
    {
        private string? _id;
        private string? _title;
        private string? _description;
        private Uri? _customeUrl;
        private Uri? _thumbUrl;
        private DateTimeOffset? _date;
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
        public Uri? CustomUrl {
            get { return _customeUrl; }
            set { _customeUrl = value; }
        }
        public Uri? ThumbUrl {
            get { return _thumbUrl; }
            set { _thumbUrl = value; }
        }
        public DateTimeOffset? Date {
            get { return _date; }
            set { _date = value; }
        }
        public ulong ViewCount {
            get { return _viewCount; }
            set { _viewCount = value; }
        }
        public ulong VideoCount {
            get { return _videoCount; }
            set { _videoCount = value; }
        }

        public Channel(string? id = null,
            string? title = null,
            string? description = null,
            Uri? customeUrl = null, Uri?
            thumbUrl = null,
            ulong viewCount = 0,
            ulong videoCount = 0)
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
