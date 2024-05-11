using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Views;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaApplication2.CustomControls;

public partial class StationControl : UserControl
{
    private MainWindow parent;
    private TrackControl? selectedTrack;
    public Station station { get; }

    public StationControl()
    {
        InitializeComponent();
    }

    public StationControl(MainWindow mainWindow, string stationName) 
    {
        InitializeComponent();

        parent = mainWindow;
        StationName.Text = stationName;

        ControlPanel.IsVisible = false;
        StationWrapper.Height = mainWindow.FindControl<WrapPanel>("Workplace").Bounds.Height;
        TracksBorder.Height = StationWrapper.Height;

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

        this.AddHandler(KeyDownEvent, StationControl_KeyDown, RoutingStrategies.Tunnel);
    }

    private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        parent.CloseStationControl(this);
    }

    public void SelectTrack(TrackControl trackControl) 
    {
        ((MainWindowViewModel)DataContext).SelectedStation = station;

        ((MainWindowViewModel)DataContext).SelectedTrack = trackControl.Track;

        selectedTrack = trackControl;

        foreach (TrackControl otherTrackControl in TracksPanel.Children) 
        {
            if (otherTrackControl != trackControl) 
            {
                otherTrackControl.Unselect();
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

    private void StationControl_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        StationWrapper.Width = Width;
        StationWrapper.Height = Height;
        if (ControlPanel.IsVisible)
        {
            TracksBorder.Height = StationWrapper.Height - ControlPanel.Height;
        }
        else
        {
            TracksBorder.Height = StationWrapper.Height;
        }
    }

    public void StationControl_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Tab || e.Key == Key.Enter)
        {
            if (selectedTrack != null)
            {
                selectedTrack.TrackControl_KeyDown(this, e);
            }
            else
            {
                if (TracksPanel.Children.Count > 0)
                {
                    selectedTrack = (TrackControl)TracksPanel.Children[0];
                    selectedTrack.Select();
                }
            }
        }
    }

    public void SelectNextTrack()
    {
        for (int i = 0; i < TracksPanel.Children.Count; i++)
        {
            if ((TrackControl)TracksPanel.Children[i] == selectedTrack)
            {
                selectedTrack.Unselect();
                selectedTrack = (TrackControl)TracksPanel.Children[(i + 1) % TracksPanel.Children.Count];
                selectedTrack.Select();
            }
        }
    }
}