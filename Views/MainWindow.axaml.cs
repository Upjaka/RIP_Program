using Avalonia.Controls;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.CustomControls;
using Avalonia.Input;

namespace AvaloniaApplication2.Views
{
    public partial class MainWindow : Window
    {
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
            var stationControl = new StationControl(this, name);
            //Workplace.Children.Add(stationControl);

            StationStateWindow stationWindow = new StationStateWindow(stationControl, (MainWindowViewModel)DataContext);
            stationWindow.Show();
        }

        private void TrackEdit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).UpdateCarsInfo();
            var trackEditing = new TrackEditingDialogWindow(this, (MainWindowViewModel)DataContext);
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

        public void CloseStationControl(StationControl stationControl)
        {
            Workplace.Children.Remove(stationControl);
        }

        public void UpdateSelectedTrack()
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            foreach (StationControl stationControl in Workplace.Children)
            {
                if (stationControl.StationName.Text == viewModel.SelectedStation.StationName)
                {
                    stationControl.UpdateTrack(viewModel.SelectedTrack);
                }
            }
        }

        private void Grid_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                
            }
        }
    }
}