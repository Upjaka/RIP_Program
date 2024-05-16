using Avalonia.Controls;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.CustomControls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using AvaloniaApplication2.Models;
using System.Diagnostics;
using System;



namespace AvaloniaApplication2.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private List<StationStateWindow> stationWindows;
        private PDFCreator PDFCreator { get; set; }

        public MainWindow()
        {
            PDFCreator = new PDFCreator();

            InitializeComponent();
            stationWindows = new List<StationStateWindow>();

            Opened += (s, e) =>
            {
                viewModel = (MainWindowViewModel)DataContext;

                viewModel.MainWindow = this;

                foreach (Station station in viewModel.Stations)
                {
                    MenuItem stationMenuItem = new MenuItem();

                    stationMenuItem.Header = station.StationName;
                    stationMenuItem.Click += (s, e) =>
                    {
                        StationStateWindow stationWindow = new StationStateWindow(station, (MainWindowViewModel)DataContext);

                        StationStateWindow? openedStationWindow = null;

                        foreach (StationStateWindow stationStateWindow in stationWindows)
                        {
                            if (stationStateWindow.Station == station)
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
                                stationWindows.Remove(stationWindow);
                                CheckStations();
                            };
                            stationWindow.Show();
                        }
                        CheckStations();
                    };

                    StationsList_MenuItem.Items.Add(stationMenuItem);
                }
            };            
        }

        private async void NewComing_Click(object? sender, RoutedEventArgs e)
        {
            OpenNewComingWindow();
        }

        private void OpenStation_Click(object? sender, RoutedEventArgs e)
        {
            string name = (sender as MenuItem).Header.ToString();
            //Workplace.Children.Add(stationControl);

            StationStateWindow stationWindow = new StationStateWindow(viewModel.GetStationByName(name), (MainWindowViewModel)DataContext);

            StationStateWindow? openedStationWindow = null;

            foreach (StationStateWindow stationStateWindow in stationWindows)
            {
                if (stationStateWindow.StationControl.Station.StationName == name)
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

        private void TrackEdit_Click(object? sender, RoutedEventArgs e)
        {
            OpenTrackEditWindow();
        }

        private void ChangeTrack_Click(object? sender, RoutedEventArgs e)
        {
            var changeTrackWindow = new ChangeTrackDialogWindow((MainWindowViewModel)DataContext);
            changeTrackWindow.ShowDialog(this);
        }

        private void DefectCodes_Click(object? sender, RoutedEventArgs e)
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
                if (stationWindow.StationControl.Station.StationName == viewModel.SelectedStation.StationName)
                {
                    stationWindow.StationControl.UpdateTrack(trackNumber);
                }
            }
        }

        public void UpdateTrack(Track track)
        {
            foreach (StationStateWindow stationWindow in stationWindows)
            {
                if (stationWindow.Station.StationId == track.StationId)
                {
                    stationWindow.UpdateTrack(track);
                }
            }
        }

        private void SaveMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            SaveChanges();
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
                    Close();
                };
                saveChangesDialogWindow.NoButton.Click += (s, e) =>
                {
                    saveChangesDialogWindow.Close();
                    viewModel.ClearChanges();
                    Close();
                };
                saveChangesDialogWindow.CancelButton.Click += (s, e) =>
                {
                    saveChangesDialogWindow.Close();
                };
                await saveChangesDialogWindow.ShowDialog<bool>(this);
            }
            CloseAllStationWindows();
        }

        private void CloseAllStationWindows()
        {
            List<StationStateWindow> stationWindowsCopy = new List<StationStateWindow>(stationWindows);

            foreach (var window in stationWindowsCopy)
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

        public void OpenTrackEditWindow()
        {
            ((MainWindowViewModel)DataContext).UpdateCarsInfo();
            var trackEditing = new TrackEditingDialogWindow(this, (MainWindowViewModel)DataContext);
            trackEditing.ShowDialog(this);
        }

        public void OpenNewComingWindow()
        {
            var newComingDialogWindow = new NewComingDialogWindow(this);
            newComingDialogWindow.DataContext = DataContext;

            newComingDialogWindow.ShowDialog(this);
        }

        public void SaveChanges()
        {
            var viewModel = (MainWindowViewModel)DataContext;
            if (viewModel.SaveChangesToDataBase())
            {
                StatusBarTextBlock.Text = "���������";
            }
        }

        private void ReportMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            ShowFieldSheet();
        }

        public void ShowFieldSheet()
        {
            PDFCreator.Create(viewModel.SelectedStation, viewModel.SelectedTrack);

            try
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = PDFCreator.TEMP_FILE_NAME,
                    UseShellExecute = true
                };
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("�� ������� ������� PDF-����: " + ex.Message);
            }
        }
    }
}