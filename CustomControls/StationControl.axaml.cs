using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Views;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2.CustomControls;

public partial class StationControl : UserControl
{
    MainWindow parent;
    Station station;

    public StationControl()
    {
        InitializeComponent();
    }

    public StationControl(MainWindow mainWindow, string stationName) 
    {
        InitializeComponent();
        parent = mainWindow;
        StationName.Text = stationName;
        Wrapper.Height = mainWindow.FindControl<WrapPanel>("Workplace").Bounds.Height;
        var dataContext = (MainWindowViewModel)mainWindow.DataContext;
        DataContext = dataContext;
        station = dataContext.GetStationByName(stationName);
        foreach (Track track in station.Tracks) 
        {
            var trackControl = new TrackControl(track, this);
            trackControl.Margin = new Thickness(10, 5, 0, 5);
            trackControl.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            trackControl.ZIndex = 0;
            TracksPanel.Children.Add(trackControl);
        }
        TracksBorder.Height = Wrapper.Height - ControlPanel.Height;
    }

    private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        parent.CloseStationControl(this);
    }

    public void SelectTrack(Track track) 
    {
        ((MainWindowViewModel)DataContext).SelectedStation = station;

        ((MainWindowViewModel)DataContext).SelectedTrack = track;

        foreach (TrackControl trackControl in TracksPanel.Children) 
        {
            if (trackControl.Track != track) 
            {
                trackControl.MakeUnselected();
            }
        }
    }

    public void UpdateTrack(Track track)
    {
        foreach (TrackControl trackControl in TracksPanel.Children)
        {
            if (trackControl.Track == track)
            {
                trackControl.UpdateTrack();
            }
        }
    }
}