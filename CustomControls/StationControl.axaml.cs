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
    public Station Station { get; }

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

        Station = dataContext.GetStationByName(stationName);
        foreach (Track track in Station.Tracks) 
        {
            var trackControl = new TrackControl(track, this);
            trackControl.Margin = new Thickness(10, 5, 0, 5);
            trackControl.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            trackControl.ZIndex = 0;
            TracksPanel.Children.Add(trackControl);
        }

        AddHandler(KeyDownEvent, StationControl_KeyDown, RoutingStrategies.Tunnel);
        AddHandler(PointerPressedEvent, StationControl_PointerPressed, RoutingStrategies.Bubble);
    }

    public void StationControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TrackControl)
        {
            selectedTrack = (TrackControl)sender;
            SelectTrack(((TrackControl)sender).Track);
        }
        e.Handled = true;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        parent.CloseStationControl(this);
    }

    private void SelectTrack(Track track) 
    {
        ((MainWindowViewModel)DataContext).SelectedTrack = track;

        foreach (TrackControl otherTrackControl in TracksPanel.Children) 
        {
            if (otherTrackControl.Track != track) 
            {
                otherTrackControl.Unselect();
            }
        }
    }

    public void UpdateTrack(Track track)
    {
        selectedTrack.UpdateTrack();
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
        switch (e.Key)
        {
            case Key.Tab:
                if (selectedTrack != null)
                {
                    selectedTrack.TrackControl_KeyDown(this, e);
                }
                else
                {
                    SelectFirstTrack();
                }
                break;

            case Key.Enter:
            case Key.Right:
            case Key.Left:
                if (selectedTrack != null)
                {
                    selectedTrack.TrackControl_KeyDown(this, e);
                }
                break;

            case Key.Down:
                if (selectedTrack == null) 
                {
                    SelectFirstTrack();
                }
                else
                {
                    SelectNextTrack();
                }
                break;

            case Key.Up:
                if (selectedTrack == null)
                {
                    SelectLastTrack();
                }
                else
                {
                    SelectPreviousTrack();
                }
                break;
        }
    }

    private void SelectFirstTrack()
    {
        if (TracksPanel.Children.Count > 0)
        {
            selectedTrack = (TrackControl)TracksPanel.Children[0];
            selectedTrack.Select();
        }
    }

    private void SelectLastTrack()
    {
        if (TracksPanel.Children.Count > 0)
        {
            selectedTrack = (TrackControl)TracksPanel.Children[TracksPanel.Children.Count - 1];
            selectedTrack.Select();
        }
    }

    private void SelectNextTrack()
    {
        int index = TracksPanel.Children.IndexOf(selectedTrack);

        int lastFocusedCarIndex = selectedTrack.Unselect();
        selectedTrack = (TrackControl)TracksPanel.Children[(index + 1) % TracksPanel.Children.Count];
        selectedTrack.Select(lastFocusedCarIndex);
    }

    private void SelectPreviousTrack()
    {
        int index = TracksPanel.Children.IndexOf(selectedTrack);

        int lastFocusedCarIndex = selectedTrack.Unselect();
        selectedTrack = (TrackControl)TracksPanel.Children[(index - 1 + TracksPanel.Children.Count) % TracksPanel.Children.Count];
        selectedTrack.Select(lastFocusedCarIndex);
    }
}