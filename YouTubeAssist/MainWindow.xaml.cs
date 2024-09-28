using System.Text;
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
using YouTubeAssist.Services;
using YouTubeAssist.UI;

namespace YouTubeAssist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SearchView searchView;
        NotificationsView notificationsView;
        AboutView aboutView;

        public MainWindow()
        {
            InitializeComponent();
            searchView = new SearchView();
            notificationsView = new NotificationsView();
            aboutView = new AboutView();
            MainContentControl.Content = searchView;

            PipeServer pipeServer = new PipeServer();
        }

        private void Search_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = searchView;
        }

        private void Notifications_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = notificationsView;
        }

        private void About_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = aboutView;
        }
    }
}