using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Views;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using Avalonia.LogicalTree;
using System.Diagnostics;

namespace AvaloniaApplication2.CustomControls;

public partial class StationControl : UserControl
{
    private static readonly int[] FONT_SIZES = { 11, 12, 12, 12, 13, 13, 14 };
    private static readonly int[] CARINFOGRID_SIZES = { 725, 750, 750, 750, 875, 875, 1000 };

    private int ZoomLevel = 1;
    private MainWindowViewModel viewModel;
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

        viewModel = (DataContext as MainWindowViewModel);

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
        foreach (TrackControl child in TracksPanel.Children)
        {
            if (child.Width + 18 > Width) Width = child.Width + 18;
        }
        Height = ControlPanel.Height + CarInfoPanel.Height + TracksBorder.Height + 10;

        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;

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
        viewModel.SelectedStation = null;
        viewModel.SelectedTrack = null;
        if (ParentWindow != null)
        {
            ParentWindow.Close();
        }
        viewModel.MainWindow.CloseStationControl(this);
    }

    private void DetachButton_Click(object? sender, RoutedEventArgs e)
    {
        DetachButton.IsVisible = false;
        AttachButton.IsVisible = true;
        StationWrapper.BorderThickness = new Thickness(0);
        TracksScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
        TracksScrollViewer.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Visible;
        (DataContext as MainWindowViewModel).MainWindow.DetachStationControl(this);
    }

    private void AttachButton_Click(object? sender, RoutedEventArgs e)
    {
        Width = TracksPanel.Children[0].Width + 18;
        TrackControl lastTrackControl = TracksPanel.Children[TracksPanel.Children.Count - 1] as TrackControl;
        Height = lastTrackControl.Bounds.Bottom + lastTrackControl.Height + 15;

        DetachButton.IsVisible = true;
        AttachButton.IsVisible = false;
        StationWrapper.BorderThickness = new Thickness(1);
        TracksScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Hidden;
        TracksScrollViewer.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Hidden;
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

            case Key.OemPlus:
                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    ZoomIn();
                }
                break;

            case Key.OemMinus:
                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    ZoomOut();
                }
                break;

            default:
                if (selectedTrack != null)
                    selectedTrack.TrackControl_KeyDown(this, e);
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
            else
            {
                scrollViewer.Offset = new Vector(scrollViewer.Offset.X + e.Delta.Y * 20, scrollViewer.Offset.Y);
            }
        }
    }

    private void ZoomInButton_Click(object? sender, RoutedEventArgs e)
    {
        ZoomIn();
    }

    private void ZoomOutButton_Click(object? sender, RoutedEventArgs e)
    {
        ZoomOut();
    }

    private void UpdateFontSize()
    {
        var carInfoTextBlocks = CarInfoGrid.GetLogicalChildren();

        foreach (var child in carInfoTextBlocks)
        {
            if (child is TextBlock textBlock)
            {
                textBlock.FontSize = FONT_SIZES[ZoomLevel];
            }
            else
            {
                foreach (var child1 in child.GetLogicalChildren())
                {
                    if (child1 is TextBlock textBlock1)
                    {
                        textBlock1.FontSize = FONT_SIZES[ZoomLevel];
                    }
                }
            }
        }
    }

    private void ZoomIn()
    {
        if (ZoomLevel != FONT_SIZES.Length - 1)
        {
            ZoomLevel++;

            CarInfoGrid.Width = CARINFOGRID_SIZES[ZoomLevel];
            UpdateFontSize();
        }
        foreach (TrackControl trackControl in TracksPanel.Children)
        {
            trackControl.ZoomIn();
        }

        TracksBorder.Width = TracksPanel.Children[0].Width + 18;
        foreach (TrackControl child in TracksPanel.Children)
        {
            if (child.Width + 18 > TracksBorder.Width) TracksBorder.Width = child.Width + 18;
        }
        if (ParentWindow == null)
        {
            Width = TracksBorder.Width;
        }
    }

    private void ZoomOut()
    {
        if (ZoomLevel != 0)
        {
            ZoomLevel--;

            CarInfoGrid.Width = CARINFOGRID_SIZES[ZoomLevel];
            UpdateFontSize();
        }
        foreach (TrackControl trackControl in TracksPanel.Children)
        {
            trackControl.ZoomOut();
        }

        TracksBorder.Width = TracksPanel.Children[0].Width + 18;
        foreach (TrackControl child in TracksPanel.Children)
        {
            if (child.Width + 18 > TracksBorder.Width) TracksBorder.Width = child.Width + 18;
        }
        if (ParentWindow == null)
        {
            Width = TracksBorder.Width;
        }
    }


    public TrackControl? GetTrackControlByPoint(TrackControl startTrack, Point point)
    {
        int currentIndex = TracksPanel.Children.IndexOf(startTrack);

        var transform = startTrack.TransformToVisual(TracksPanel);

        var position = transform.Value.Transform(point);

        foreach (TrackControl trackControl in TracksPanel.Children)
        {
            if (HitTest(trackControl, position)) return trackControl;
        }

        return null;
    }

    public bool HitTest(TrackControl trackControl, Point point)
    {
        if (point.X > trackControl.Bounds.Left && point.X < trackControl.Bounds.Right)
        {
            if (point.Y < trackControl.Bounds.Bottom && point.Y > trackControl.Bounds.Top)
            {
                return true;
            }
        }
        return false;
    }
}