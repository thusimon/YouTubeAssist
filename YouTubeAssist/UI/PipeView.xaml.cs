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
using YouTubeAssist.Services;

namespace YouTubeAssist.UI
{
    /// <summary>
    /// Interaction logic for PipeView.xaml
    /// </summary>
    public partial class PipeView : UserControl, INotifyPropertyChanged
    {
        private string messageIncome = "";
        private string messageOutput = "";
        PipeServer? pipeServer = null;
        public event PropertyChangedEventHandler PropertyChanged;

        public string MessageIncome
        {
            get
            {
                return messageIncome;
            }
            set
            {
                messageIncome = value;
                OnPropertyChanged();
            }
        }

        public string MessageOutput
        {
            get
            {
                return messageOutput;
            }
            set
            {
                messageOutput = value;
                OnPropertyChanged();
            }
        }
        public PipeView()
        {
            pipeServer = new PipeServer(this);
            InitializeComponent();
            DataContext = this;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            pipeServer.sendMessage(MessageOutput);
        }
    }
}
