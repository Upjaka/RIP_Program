using Avalonia.Controls;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2.Views
{
    public partial class MainWindow : Window
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\путь_к_вашей_базе_данных.accdb";

        public string receivedButtonContent { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void UpdateButtonContent(string content)
        {
            // Assuming there is a button named "receivedButton"
            receivedButtonContent = content;
        }

        private async void NewComing_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var newComingDialogWindow = new NewComingDialogWindow(this);
            newComingDialogWindow.DataContext = this.DataContext;

            await newComingDialogWindow.ShowDialog(this);
        }

        private void OpenStation_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string name = (sender as MenuItem).Header.ToString();
            var stationWindow = new StationStateWindow((MainWindowViewModel)this.DataContext, name);
            StationStateWindow stateWindow = new StationStateWindow((MainWindowViewModel)this.DataContext, name);
            stateWindow.Show();
        }

        private void TrackEdit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var trackEditing = new TrackEditingDialogWindow((MainWindowViewModel)DataContext);
            trackEditing.ShowDialog(this);
        }

        private void ChangeTrack_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var changeTrackWindow = new ChangeTrackDialogWindow((MainWindowViewModel)DataContext);
            changeTrackWindow.ShowDialog(this);
        }

        private void DefectCodes_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var defectCodesWindow = new DefectCodesDialogWindow((MainWindowViewModel)DataContext);
            defectCodesWindow.ShowDialog(this);
        }
    }
}