using EddnParser.EDDN;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static EddnParser.Commanders;

namespace EddnParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Property Changed
        // Declare the event
        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            if (Dispatcher.CheckAccess())
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                return;
            }

            Dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }
        #endregion

        private ObservableCollection<Commanders> commaders = new();
        public ObservableCollection<Commanders> Commanders { get => commaders; set { commaders = value; OnPropertyChanged(); } }

        private ObservableCollection<CommanderMessages> messages = new();
        public ObservableCollection<CommanderMessages> Messages123 { get => messages; set { messages = value; OnPropertyChanged(); } }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ProcessLogs_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(input.Text))
            {
                MessageBox.Show(this, "Enter a system!", "WTF??",MessageBoxButton.OK);
                return;
            }

            ProcessLogs.Content = "Processing Logs.....";

            var ret = await EddnHistoryParser.ParseHistory(input.Text);

            if(ret is not null && ret.Count > 0)
            {
                Commanders.Clear();

                foreach (var cmdr in ret)
                {
                    Commanders.Add(cmdr);
                }

                IdGrid.SelectedIndex = 0;

                ProcessLogs.Content = "Process Logs";
                return;
            }
            ProcessLogs.Content = "Process Logs";
            MessageBox.Show(this, "We aint found shit!", "Lord Helmet", MessageBoxButton.OK);
        }

        private void IdGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(IdGrid.SelectedItem is Commanders cmdrData)
            {
                Messagestring.ItemsSource = cmdrData.Messages;
                Messagestring.Items.Refresh();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommanderMessages? obj = ((FrameworkElement)sender).DataContext as CommanderMessages;

            if(obj is not null)
            {
                obj.SearchInara(null, null);
            }
        }
    }
}
