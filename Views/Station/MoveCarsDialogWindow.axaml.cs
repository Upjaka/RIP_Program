using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Models;
using Avalonia.Input;

namespace AvaloniaApplication2;

public partial class MoveCarsDialogWindow : Window
{
    private MainWindowViewModel viewModel;

    private Station? SelectedStation = null;
    private Track? SelectedTrack = null;

    public MoveCarsDialogWindow(Station destStation = null, Track destTrack = null)
    {
        InitializeComponent();

        Opened += (s, e) =>
        {
            viewModel = (MainWindowViewModel)Owner.DataContext;
            DataContext = viewModel;

            Title = $"Перем. из: { viewModel.SelectedStation.StationName } путь { viewModel.SelectedTrack.TrackNumber }";

            int i = 1;

            foreach (Station station in viewModel.Stations)
            {
                Border stationBorder = new Border()
                {                  
                    Name = "Station_" + i,
                    Classes = { "StationBorder" }
                };

                TextBlock stationNumberTextBlock = new TextBlock()
                {
                    Text = i.ToString(),
                    Classes = { "StationNumber" },
                    Width = 18,
                    Margin = new Thickness(2, 0, 0, 0)
                };

                Border stationNumberBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 0),
                    Child = stationNumberTextBlock
                };

                TextBlock stationNameTextBlock = new TextBlock()
                {
                    Text = station.StationName,
                    Classes = { "StationName" },
                    Margin = new Thickness(6, 0, 0, 0)
                };

                StackPanel stationPanel = new StackPanel()
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Classes = { "StationPanel" }
                };

                stationPanel.Children.Add(stationNumberBorder);
                stationPanel.Children.Add(stationNameTextBlock);

                stationBorder.Child = stationPanel;

                stationBorder.PointerPressed += (s, e) =>
                {
                    e.Handled = true;
                    StationBorderPointerPressedHadler(s as Border, station);
                    SelectedStation = station;
                };

                StationsListPanel.Children.Add(stationBorder);

                if (destStation != null && station == destStation)
                {
                    stationBorder.Classes.Add("Selected");
                    UpdateTracksList(destStation, destTrack);
                    SelectedStation = destStation;
                }
                i++;
            }            
        };

        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.None)
            {
                MoveCars();
            }
        };
    }

    private void UpdateTracksList(Station selectedStation, Track selectedTrack = null)
    {
        TracksListPanel.Children.Clear();

        foreach (Track track in selectedStation.Tracks)
        {
            TextBlock trackNumberTextBlock = new TextBlock()
            {
                Text = track.TrackNumber.ToString(),
                Classes = { "TrackNumber" },
                Margin = new Thickness(2, 0, 0, 0)
            };

            Border trackBorder = new Border()
            {
                Name = "TrackBorder_" + track.TrackNumber,
                Classes = { "TrackNumberBorder" },
                Child = trackNumberTextBlock
            };

            trackBorder.PointerPressed += (s, e) =>
            {
                e.Handled = true;
                TrackBorderPointerPressedHadler(trackBorder);
                SelectedTrack = track;
            };

            TracksListPanel.Children.Add(trackBorder);

            if (selectedTrack == track)
            {
                trackBorder.Classes.Add("Selected");
                SelectedTrack = track;
            }
        }
    }

    private void StationBorderPointerPressedHadler(Border stationBorder, Station station)
    {
        foreach (Border sb in StationsListPanel.Children)
        {
            sb.Classes.Remove("Selected");
        }
        stationBorder.Classes.Add("Selected");

        UpdateTracksList(station);
    }

    private void TrackBorderPointerPressedHadler(Border trackBorder)
    {
        foreach (Border tb in TracksListPanel.Children)
        {
            tb.Classes.Remove("Selected");
        }
        trackBorder.Classes.Add("Selected");
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MoveCars();
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private void MoveCars()
    {
        if (SelectedStation != null && SelectedTrack != null)
        {

            if (!viewModel.MoveCars(SelectedTrack, MovingSideCheckBox.IsChecked ?? true))
            {
                LackOfSpaceOnTrackDialogWindow lackOfSpaceOnTrackWindow = new LackOfSpaceOnTrackDialogWindow();
                lackOfSpaceOnTrackWindow.ShowDialog(this);
            }
            else
            {
                viewModel.MainWindow.UpdateTrack(SelectedTrack);
                viewModel.MainWindow.UpdateTrack(viewModel.SelectedTrack);
                Close();
            }
        }
    }
}