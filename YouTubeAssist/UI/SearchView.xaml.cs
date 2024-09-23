using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YouTubeAssist.API;

namespace YouTubeAssist.UI
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl, INotifyPropertyChanged
    {
        YouTubeAPI youTubeAPI;
        int searchStatus; // -1: no result; 0: searching; 1: has result
        Channel channel;
        string _channelId;
        string _channelTitle;
        public event PropertyChangedEventHandler PropertyChanged;

        public SearchView()
        {
            InitializeComponent();
            youTubeAPI = new YouTubeAPI();
            searchStatus = 1;
            channel = new Channel();
            DataContext = this;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async void Search_button_Click(object sender, RoutedEventArgs e)
        {
            SearchStatus = 0;
            search_textBox.IsReadOnly = true;
            string handle = search_textBox.Text;
            Channel? channel = await youTubeAPI.SearchChannel(handle);
            search_textBox.IsReadOnly = false;
            SearchStatus = channel == null ? -1 : 1;
            if (channel != null) {
                Channel = channel;
            }
            else
            {
                Channel = new Channel();
            }
        }

        public int SearchStatus
        { 
            get
            { 
                return searchStatus;
            }
            set
            {
                searchStatus = value;
                OnPropertyChanged();
            }
        }

        public Channel Channel
        {
            get
            {
                return channel;
            }
            set
            {
                channel = value;
                OnPropertyChanged();
            }
        }
        public string ChannelID
        {
            get
            {
                return _channelId;
            }
            set
            {
                _channelId = value;
                OnPropertyChanged();
            }
        }

        public string ChannelTitle
        {
            get
            {
                return _channelTitle;
            }
            set
            {
                _channelTitle = value;
                OnPropertyChanged();
            }
        }

        //public string ChannelDescription
        //{
        //    get
        //    {
        //        return channelResult!.Description;
        //    }
        //}

        //public string ChannelUrl
        //{
        //    get
        //    {
        //        return "https://www.youtube.com/" + channelResult!.CustomUrl;
        //    }
        //}

        //public string ChannelThumbUrl
        //{
        //    get
        //    {
        //        return channelResult!.ThumbUrl;
        //    }
        //}

        //public ulong ChannelViewCount
        //{
        //    get
        //    {
        //        return channelResult!.ViewCount;
        //    }
        //}

        //public ulong ChannelVideoCount
        //{
        //    get
        //    {
        //        return channelResult!.VideoCount;
        //    }
        //}
    }
}
