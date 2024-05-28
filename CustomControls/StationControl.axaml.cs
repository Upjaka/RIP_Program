using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Views;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace AvaloniaApplication2.CustomControls;

public partial class StationControl : UserControl
{
    public StationStateWindow ParentWindow { get; set; }

    private TrackControl? selectedTrack;
    public TrackControl? SelectedTrack { get { return selectedTrack; } }
    public Station Station { get; }

    private Car _lastFocusedCar;
    public Car LastFocusedCar
    {
        get => _lastFocusedCar;
        set
        {
            _lastFocusedCar = value;
            CarSerialNumberTextBlock.Text = (value == null) ? "" : _lastFocusedCar.SerialNumber.ToString();
            CarNumberTextBlock.Text = (value == null) ? "" : _lastFocusedCar.CarNumber;
            CarCargoTextBlock.Text = (value == null) ? "" : _lastFocusedCar.Cargo;
            CarDefectTextBlock.Text = (value == null) ? "" : _lastFocusedCar.DefectCodes;
            CarProductTextBlock.Text = (value == null) ? "" : _lastFocusedCar.Product;
            CarArrivalTextBlock.Text = (value == null) ? "" : _lastFocusedCar.Arrival.ToString("dd/MM/yy");
            CarDowntimeTextBlock.Text = (value == null) ? "" : (DateTime.Now - _lastFocusedCar.Arrival).Days.ToString() + " дн";
        }
    }


    public StationControl()
    {
        InitializeComponent();
    }

    public StationControl(MainWindowViewModel dataContext, Station station)
    {
        DataContext = dataContext;

        Station = station;

        InitializeComponent();

        AttachButton.IsVisible = false;

        StationName.Text = Station.StationName;

        //ControlPanel.IsVisible = false;
        //StationWrapper.Height = mainWindow.FindControl<WrapPanel>("Workplace").Bounds.Height;
        //TracksBorder.Height = StationWrapper.Height;

        foreach (Track track in Station.Tracks)
        {
            var trackControl = new TrackControl(track, this);
            trackControl.Margin = new Thickness(10, 5, 0, 5);
            trackControl.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            trackControl.ZIndex = 0;
            TracksPanel.Children.Add(trackControl);
        }

        Width = TracksPanel.Children[0].Width + 18;        
        Height = ControlPanel.Height + CarInfoPanel.Height + TracksBorder.Height + 10;
        

        AddHandler(KeyDownEvent, StationControl_KeyDown, RoutingStrategies.Tunnel);
        AddHandler(PointerPressedEvent, StationControl_PointerPressed, RoutingStrategies.Bubble);
    }

    public void StationControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TrackControl trackControl)
        {
            selectedTrack = trackControl;
            UnselectOtherTracks(trackControl.Track);
        }
        (DataContext as MainWindowViewModel).SelectedStation = Station;
        e.Handled = true;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ParentWindow == null)
        {
            (DataContext as MainWindowViewModel).MainWindow.CloseStationControl(this);
        }
        else
        {
            ParentWindow.Close();
        }
    }

    private void DetachButton_Click(object? sender, RoutedEventArgs e)
    {
        DetachButton.IsVisible = false;
        AttachButton.IsVisible = true;
        (DataContext as MainWindowViewModel).MainWindow.DetachStationControl(this);
    }

    private void AttachButton_Click(object? sender, RoutedEventArgs e)
    {
        Width = TracksPanel.Children[0].Width + 18;
        TrackControl lastTrackControl = TracksPanel.Children[TracksPanel.Children.Count - 1] as TrackControl;
        Height = lastTrackControl.Bounds.Bottom + lastTrackControl.Height + 15;

        DetachButton.IsVisible = true;
        AttachButton.IsVisible = false;
        ParentWindow.Close();
        ParentWindow.StationPanel.Children.Remove(this);
        (DataContext as MainWindowViewModel).MainWindow.AttachStationControl(this);
    }

    private void UnselectOtherTracks(Track track) 
    {
        foreach (TrackControl otherTrackControl in TracksPanel.Children) 
        {
            if (otherTrackControl.Track != track) 
            {
                otherTrackControl.Unselect();
            }
        }
    }

    public void UpdateTrack(int trackNumber)
    {
        foreach (TrackControl trackControl in TracksPanel.Children)
        {
            if (trackControl.Track.TrackNumber == trackNumber)
            {
                trackControl.UpdateTrack();
            }
        }
    }

    private void StationControl_SizeChanged(object? sender, SizeChangedEventArgs e)
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

    public void StationControl_KeyDown(object? sender, KeyEventArgs e)
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
            selectedTrack.Select(0);
        }
    }

    private void SelectLastTrack()
    {
        if (TracksPanel.Children.Count > 0)
        {
            selectedTrack = (TrackControl)TracksPanel.Children[TracksPanel.Children.Count - 1];
            selectedTrack.Select(0);

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

    private void ScrollViewer_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            if (CarInfoGrid.Bounds.Width > scrollViewer.Bounds.Width)
            {
                scrollViewer.Offset = new Vector(scrollViewer.Offset.X - e.Delta.Y * 20, scrollViewer.Offset.Y);
            }
        }        
    }

    public void Dettach()
    {

    }
}