using Avalonia.Controls;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.CustomControls;
using Avalonia.Input;
using System.Collections.Generic;
using AvaloniaApplication2.Models;

namespace AvaloniaApplication2.Views
{
    public partial class MainWindow : Window
    {
        private List<StationStateWindow> stationWindows;

        public MainWindow()
        {
            InitializeComponent();
            stationWindows = new List<StationStateWindow>();
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

            StationStateWindow? openedStationWindow = null;

            foreach (StationStateWindow stationStateWindow in stationWindows)
            {
                if (stationStateWindow.StaionControl.Station.StationName == name)
                {
                    openedStationWindow = stationStateWindow;
                }
            }
            if (openedStationWindow != null)
            {
                openedStationWindow.Activate();
            }
            else
            {
                stationWindows.Add(stationWindow);
                stationWindow.Closed += (s, e) =>
                {
                    CheckStations();
                };
                stationWindow.Show();
            }
            CheckStations();
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

        public void UpdateSelectedTrack(int trackNumber)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            foreach (StationStateWindow stationWindow in stationWindows)
            {
                if (stationWindow.StaionControl.Station.StationName == viewModel.SelectedStation.StationName)
                {
                    stationWindow.StaionControl.UpdateTrack(trackNumber);
                }
            }
        }

        private void SaveMenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            if (viewModel.SaveChangesToDataBase())
            {
                StatusBarTextBlock.Text = "Сохранено";
            }
        }

        private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            if (((MainWindowViewModel)DataContext).HasUnsavedChanges)
            {
                e.Cancel = true;

                SaveChangesDialogWindow saveChangesDialogWindow = new SaveChangesDialogWindow((MainWindowViewModel)DataContext);
                saveChangesDialogWindow.YesButton.Click += (s, e) =>
                {
                    ((MainWindowViewModel)DataContext).SaveChangesToDataBase();
                    saveChangesDialogWindow.Close();
                    CloseAllStationWindows();
                    Close();
                };
                saveChangesDialogWindow.NoButton.Click += (s, e) =>
                {
                    saveChangesDialogWindow.Close();
                };
                saveChangesDialogWindow.CancelButton.Click += (s, e) =>
                {
                    saveChangesDialogWindow.Close();
                };
                await saveChangesDialogWindow.ShowDialog<bool>(this);
            }
        }

        private void CloseAllStationWindows()
        {
            foreach (StationStateWindow window in stationWindows)
            {
                window.Close();
            }
        }

        public void CheckStations()
        {
            if (stationWindows.Count > 0)
            {
                NewComing_MenuItem.IsEnabled = true;
                TrackEdit_MenuItem.IsEnabled = true;
            }
        }
    }
}