using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
        string searchHandle = "";
        Channel channel;
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

        public string SearchHandle
        {
            get
            {
                return searchHandle;
            }
            set
            {
                searchHandle = value;
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

        public async void Search_button_Click(object sender, RoutedEventArgs e)
        {
            SearchStatus = 0;
            search_textBox.IsReadOnly = true;
            string handle = search_textBox.Text;
            if (string.IsNullOrWhiteSpace(handle))
            {
                Debug.WriteLine("empty handle, do nothing");
                return;
            }
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {

        }
    }
}
