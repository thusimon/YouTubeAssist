using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public partial class SearchView : UserControl
    {
        YouTubeAPI youTubeAPI;
        public SearchView()
        {
            InitializeComponent();
            youTubeAPI = new YouTubeAPI();
        }

        public async void Search_button_Click(object sender, RoutedEventArgs e)
        {
            search_textBox.IsReadOnly = true;
            string handle = search_textBox.Text;

            Tuple<List<string>, List<ulong>> result = await youTubeAPI.SearchChannel(handle);
            search_textBox.IsReadOnly = false;
        }
    }
}
