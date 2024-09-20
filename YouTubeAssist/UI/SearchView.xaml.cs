using System;
using System.Collections.Generic;
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
        Tuple<List<string>, List<ulong>>? searchResult;
        public event PropertyChangedEventHandler PropertyChanged;

        public SearchView()
        {
            InitializeComponent();
            youTubeAPI = new YouTubeAPI();
            searchStatus = 1;
            searchResult = null;
            DataContext = this;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async void Search_button_Click(object sender, RoutedEventArgs e)
        {
            searchStatus = 0;
            search_textBox.IsReadOnly = true;
            string handle = search_textBox.Text;
            searchResult = await youTubeAPI.SearchChannel(handle);
            search_textBox.IsReadOnly = false;
            SearchStatus = searchResult == null ? -1 : 1;
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

        public string ChannelID
        {
            get
            {
                return searchResult?.Item1?[0] ?? "TEMP";
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public string ChannelTitle
        {
            get
            {
                return searchResult?.Item1?[1] ?? "";
            }
        }

        public string ChannelDescription
        {
            get
            {
                return searchResult?.Item1?[2] ?? "";
            }
        }

        public string ChannelUrl
        {
            get
            {
                return searchResult?.Item1?[3] ?? "";
            }
        }

        public string ChannelIcon
        {
            get
            {
                return searchResult?.Item1?[4] ?? "";
            }
        }

        public ulong ChannelViewCount
        {
            get
            {
                return searchResult?.Item2?[0] ?? 0;
            }
        }

        public ulong ChannelVideoCount
        {
            get
            {
                return searchResult?.Item2?[1] ?? 0;
            }
        }
    }
}
