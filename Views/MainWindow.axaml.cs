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
        public List<StationStateWindow> OpenedStationStateWindows;
        private PDFCreator PDFCreator { get; set; }

        public MainWindow()
        {
            PDFCreator = new PDFCreator();

            InitializeComponent();
            OpenedStationStateWindows = new List<StationStateWindow>();

            Closed += (s, e) =>
            {
                PDFCreator.Delete();

                foreach (KeyValuePair<Station, StationControl> kvp in viewModel.OpenedStationControls)
                {
                    if (kvp.Value.ParentWindow is StationStateWindow window)
                    {
                        window.Close();
                    }
                }
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
                    stationMenuItem.Click += (s, e) => OpenStation_Click(s, e);

                    StationsList_MenuItem.Items.Add(stationMenuItem);
                }

                SaveMenuItem.IsEnabled = viewModel.IsOperator;
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
            Station station = viewModel.GetStationByName(name);

            if (!viewModel.OpenedStationControls.Keys.Contains(station))
            {
                StationControl stationControl = new StationControl(DataContext as MainWindowViewModel, station);

                Workplace.Children.Add(stationControl);

                viewModel.OpenedStationControls.Add(station, stationControl);

                viewModel.SelectedStation = station;
            }
        }

        public void DetachStationControl(StationControl stationControl)
        {
            Workplace.Children.Remove(stationControl);
            //stationControl.ControlPanel.IsVisible = false;
            Station station = stationControl.Station;

            StationStateWindow stationWindow = new StationStateWindow(stationControl, (MainWindowViewModel)DataContext);

            stationControl.ParentWindow = stationWindow;

            OpenedStationStateWindows.Add(stationWindow);
            UpdateWindowsMenuItem();
            viewModel.OpenedStationControls[station] = stationControl;
            stationWindow.Show();
        }

        public void AttachStationControl(StationControl stationControl)
        {
            viewModel.OpenedStationControls[stationControl.Station] = stationControl;
            OpenedStationStateWindows.Remove(stationControl.ParentWindow);
            UpdateWindowsMenuItem();
            stationControl.ControlPanel.IsVisible = true;
            stationControl.ParentWindow = null;
            Workplace.Children.Add(stationControl);            
        }

        public void CloseStationControl(StationControl stationControl)
        {
            if (stationControl.ParentWindow != null)
            {
                OpenedStationStateWindows.Remove(stationControl.ParentWindow);
            }
            Workplace.Children.Remove(stationControl);
            viewModel.OpenedStationControls.Remove(stationControl.Station);
            SetMenuItemsEnabling();
            UpdateWindowsMenuItem();
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
            defectCodesWindow.Show();
        }

        public void UpdateTrack(Track track)
        {
            foreach (Station station in viewModel.OpenedStationControls.Keys)
            {
                if (station.StationId == track.StationId)
                {
                    viewModel.OpenedStationControls[station].UpdateTrack(track.TrackNumber);
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
            List<StationStateWindow> stationWindowsCopy = new List<StationStateWindow>(OpenedStationStateWindows);

            foreach (var window in stationWindowsCopy)
            {
                window.Close();
            }
        }

        public void OpenTrackEditWindow()
        {
            if (viewModel.IsOperator)
            {
                ((MainWindowViewModel)DataContext).UpdateCarsInfo();
                var trackEditing = new TrackEditingDialogWindow(this, (MainWindowViewModel)DataContext);
                trackEditing.ShowDialog(this);
            }           
        }

        public void OpenNewComingWindow()
        {
            if (viewModel.IsOperator)
            {
                var newComingDialogWindow = (viewModel.SelectedTrack == null) ? new NewComingDialogWindow(this) : new NewComingDialogWindow(this, viewModel.SelectedTrack.TrackNumber);
                newComingDialogWindow.DataContext = DataContext;

                newComingDialogWindow.ShowDialog(this);
            }            
        }

        public void OpenMovingCarsWindow(Station destStation = null, Track destTrack = null)
        {
            if (viewModel.IsOperator)
            {
                MoveCarsDialogWindow moveCarsDialogWindow = new MoveCarsDialogWindow(destStation, destTrack);

                moveCarsDialogWindow.ShowDialog(this);
            }            
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

        /**
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
        */

        private void UpdateWindowsMenuItem()
        {
            Windows_MenuItem.Items.Clear();

            foreach (StationStateWindow stationWindow in OpenedStationStateWindows)
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
            if (viewModel.SelectedTrack != null)
            {
                FieldSheet_MenuItem.IsEnabled = true;
                NewComing_MenuItem.IsEnabled = viewModel.IsOperator;
                TrackEdit_MenuItem.IsEnabled = viewModel.IsOperator;             
                MoveCars_MenuItem.IsEnabled = viewModel.IsOperator;
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