using Avalonia.Controls;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.CustomControls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using AvaloniaApplication2.Models;
using System.Diagnostics;
using System;
using Avalonia.Input;



namespace AvaloniaApplication2.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        public List<StationStateWindow> StationWindows;
        private PDFCreator PDFCreator { get; set; }

        public MainWindow()
        {
            PDFCreator = new PDFCreator();

            InitializeComponent();
            StationWindows = new List<StationStateWindow>();

            Closed += (s, e) =>
            {
                PDFCreator.Delete();
            };

            Opened += async (s, e) =>
            {
                viewModel = (MainWindowViewModel)DataContext;

                viewModel.MainWindow = this;
                
                LoginWindow loginWindow = new LoginWindow();
                await loginWindow.ShowDialog<bool>(this);
                if (!viewModel.IsLoggedIn)
                {
                    Close();
                }
                else if (!viewModel.IsOperator)
                {
                    Title += " (������ ��� ������)";
                }

                viewModel.ConnectToDefectCodesDatabase();

                viewModel.ConnectToStationsDatabase();

                foreach (Station station in viewModel.Stations)
                {
                    MenuItem stationMenuItem = new MenuItem();

                    stationMenuItem.Header = station.StationName;
                    stationMenuItem.Click += (s, e) =>
                    {
                        string name = (s as MenuItem).Header.ToString();
                        Station station = viewModel.GetStationByName(name);

                        if (!viewModel.OpenedStations.Keys.Contains(station))
                        {
                            StationControl stationControl = new StationControl(DataContext as MainWindowViewModel, station);

                            Workplace.Children.Add(stationControl);

                            viewModel.OpenedStations.Add(station, stationControl);
                        }
                    };

                    StationsList_MenuItem.Items.Add(stationMenuItem);
                }

            };

            AddHandler(KeyDownEvent, MainWindow_KeyDown, RoutingStrategies.Tunnel);
        }

        private void LoginWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void NewComing_Click(object? sender, RoutedEventArgs e)
        {
            OpenNewComingWindow();
        }

        private void OpenStation_Click(object? sender, RoutedEventArgs e)
        {
            string name = (sender as MenuItem).Header.ToString();

            StationStateWindow? stationWindow = null;
            foreach (StationStateWindow stationStateWindow in StationWindows)
            {
                if (stationStateWindow.Station.StationName == name)
                    stationWindow = stationStateWindow;
            }


            if (stationWindow == null)
            {
                Station station = viewModel.GetStationByName(name);

                StationControl stationControl = new StationControl(DataContext as MainWindowViewModel, station);

                Workplace.Children.Add(stationControl);
            }
            
            /**
            StationStateWindow stationWindow = new StationStateWindow(viewModel.GetStationByName(name), (MainWindowViewModel)DataContext);

            StationStateWindow? openedStationWindow = null;

            foreach (StationStateWindow stationStateWindow in StationWindows)
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
                StationWindows.Add(stationWindow);
                stationWindow.Show();
            }
            */
        }

        public void DetachStationControl(StationControl stationControl)
        {
            Workplace.Children.Remove(stationControl);
            //stationControl.ControlPanel.IsVisible = false;
            Station station = stationControl.Station;

            StationStateWindow stationWindow = new StationStateWindow(stationControl, (MainWindowViewModel)DataContext);

            stationControl.ParentWindow = stationWindow;

            //StationWindows.Add(stationWindow);
            viewModel.OpenedStations[station] = stationControl;
            stationWindow.Show();
        }

        public void AttachStationControl(StationControl stationControl)
        {
            stationControl.ControlPanel.IsVisible = true;
            stationControl.ParentWindow = null;
            Workplace.Children.Add(stationControl);
        }

        public void CloseStationControl(StationControl stationControl)
        {
            Workplace.Children.Remove(stationControl);
            viewModel.OpenedStations.Remove(stationControl.Station);
            SetMenuItemsEnabling();
        }

        private void TrackEdit_Click(object? sender, RoutedEventArgs e)
        {
            OpenTrackEditWindow();
        }

        private void MoveCars_Click(object? sender, RoutedEventArgs e)
        {
            OpenMovingCarsWindow();
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

        public void UpdateTrack(Track track)
        {
            foreach (Station station in viewModel.OpenedStations.Keys)
            {
                if (station.StationId == track.StationId)
                {
                    viewModel.OpenedStations[station].UpdateTrack(track.TrackNumber);
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
            List<StationStateWindow> stationWindowsCopy = new List<StationStateWindow>(StationWindows);

            foreach (var window in stationWindowsCopy)
            {
                window.Close();
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

        public void OpenMovingCarsWindow()
        {
            MoveCarsDialogWindow moveCarsDialogWindow = new MoveCarsDialogWindow();

            moveCarsDialogWindow.ShowDialog(this);
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
                Debug.WriteLine("�� ������� ������� PDF-����: " + ex.Message);
            }
        }

        private void WindowsMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            if (StationWindows.Count > 0)
            {
                Windows_MenuItem.Items.Clear();

                foreach (StationStateWindow stationWindow in StationWindows)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Click += (s, e) =>
                    {
                        stationWindow.Activate();
                    };
                    menuItem.Header = stationWindow.StationControl.Station.StationName;

                    Windows_MenuItem.Items.Add(menuItem);
                }
                Windows_MenuItem.UpdateLayout();
            }
        }

        private void Windows_MenuItem_SubmenuOpened(object? sender, RoutedEventArgs e)
        {
            if (StationWindows.Count > 0)
            {
                Windows_MenuItem.Items.Clear();

                foreach (StationStateWindow stationWindow in StationWindows)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Click += (s, e) =>
                    {
                        stationWindow.Activate();
                    };
                    menuItem.Header = stationWindow.StationControl.Station.StationName;

                    Windows_MenuItem.Items.Add(menuItem);
                }
            }
        }

        private void Dispatcher_MenuItem_SubmenuOpened(object? sender, RoutedEventArgs e)
        {
            SetMenuItemsEnabling();
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (Workplace.Children.Count > 0)
            {
                foreach (StationControl stationControl in Workplace.Children)
                {
                    if (stationControl.Station == viewModel.SelectedStation)
                    {
                        stationControl.StationControl_KeyDown(this, e);
                    }
                }
            }
        }

        private void ExitMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        public void SetMenuItemsEnabling()
        {
            if (viewModel.SelectedTrack != null && viewModel.IsOperator)
            {
                NewComing_MenuItem.IsEnabled = true;
                TrackEdit_MenuItem.IsEnabled = true;
                FieldSheet_MenuItem.IsEnabled = true;
                MoveCars_MenuItem.IsEnabled = true;
            }
            else
            {
                NewComing_MenuItem.IsEnabled = false;
                TrackEdit_MenuItem.IsEnabled = false;
                FieldSheet_MenuItem.IsEnabled = false;
                MoveCars_MenuItem.IsEnabled = false;
            }
        }
    }
}