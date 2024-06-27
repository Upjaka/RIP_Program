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
using Avalonia.Controls.Primitives;
using System.Collections.Generic;

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
    private List<TrackControl> trackControls;
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

        StationNameTextBlock.Text = Station.StationName;

        //ControlPanel.IsVisible = false;
        //StationWrapper.Height = mainWindow.FindControl<WrapPanel>("Workplace").Bounds.Height;
        //TracksBorder.Height = StationWrapper.Height;

        StationSchema schema = new StationSchema(Station.StationName);
        trackControls = new List<TrackControl>();

        for (var i = 0; i < schema.Rows; i++)
        {
            var row = schema.schema[i];
            RowDefinition rowDefinition = new RowDefinition { Height = GridLength.Auto };
            TracksGrid.RowDefinitions.Add(rowDefinition);

            StackPanel rowPanel = new StackPanel()
            {
                Margin = new Thickness(0),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                [Grid.RowProperty] = i,
                [Grid.ColumnProperty] = 0,
                Orientation = Avalonia.Layout.Orientation.Horizontal,
            };

            for (var j = 0; j < row.Count; j++)
            {
                var trackControl = new TrackControl(Station.GetTrackByNumber(row[j]), this)
                {
                    Margin = new Thickness(10, 5, 0, 5),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                    ZIndex = 0,
                };

                rowPanel.Children.Add(trackControl);
                trackControls.Add(trackControl);
            }

            TracksGrid.Children.Add(rowPanel);
        }

        /**
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
        */
        Height = ControlPanel.Height + CarInfoPanel.Height + TracksBorder.Height + 10;

        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;

        AddHandler(KeyDownEvent, StationControl_KeyDown, RoutingStrategies.Tunnel);
        AddHandler(PointerPressedEvent, StationControl_PointerPressed, RoutingStrategies.Bubble);

        DetachedFromVisualTree += (s, e) =>
        {
            viewModel.SelectedStation = null;
            viewModel.SelectedTrack = null;
            if (ParentWindow != null)
            {
                ParentWindow.Close();
            }
            viewModel.MainWindow.CloseStationControl(this);
        };
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
        CloseButton.IsVisible = false;
        StationWrapper.BorderThickness = new Thickness(0);
        TracksScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        TracksScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        StationNameTextBlock.IsVisible = false;
        (DataContext as MainWindowViewModel).MainWindow.DetachStationControl(this);
    }

    private void AttachButton_Click(object? sender, RoutedEventArgs e)
    {
        Width = TracksGrid.Children[0].Width + 18;
        TrackControl lastTrackControl = (TracksGrid.Children[TracksGrid.Children.Count - 1] as StackPanel).Children[0] as TrackControl;

        var transform = lastTrackControl.TransformToVisual(StationWrapper);

        var bottomLeft = transform.Value.Transform(new Point(0, lastTrackControl.Bounds.Height));

        Height = bottomLeft.Y + 15;

        DetachButton.IsVisible = true;
        AttachButton.IsVisible = false;
        CloseButton.IsVisible = true;
        StationNameTextBlock.IsVisible = true;
        StationWrapper.BorderThickness = new Thickness(1);
        TracksScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        TracksScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        ParentWindow.Close();
        ParentWindow.StationPanel.Children.Remove(this);
        (DataContext as MainWindowViewModel).MainWindow.AttachStationControl(this);
    }

    private void UnselectOtherTracks(Models.Track track)
    {
        foreach (StackPanel trackPanel in TracksGrid.Children)
        {
            foreach (TrackControl otherTrackControl in trackPanel.Children)
            {
                if (otherTrackControl.Track != track)
                {
                    otherTrackControl.Unselect();
                }
            }
        }
    }

    public void UpdateTrack(int trackNumber)
    {
        foreach (StackPanel trackPanel in TracksGrid.Children)
        {
            foreach (TrackControl trackControl in trackPanel.Children)
            {
                if (trackControl.Track.TrackNumber == trackNumber)
                {
                    trackControl.UpdateTrack();
                }
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
        if (TracksGrid.Children.Count > 0)
        {
            selectedTrack = (TracksGrid.Children[0] as StackPanel).Children[0] as TrackControl;
            selectedTrack.Select(0);
        }
    }

    private void SelectLastTrack()
    {
        if (TracksGrid.Children.Count > 0)
        {
            StackPanel lastStackPanel = TracksGrid.Children[TracksGrid.Children.Count - 1] as StackPanel;
            selectedTrack = lastStackPanel.Children[lastStackPanel.Children.Count - 1] as TrackControl;
            selectedTrack.Select(0);
        }
    }

    /**
    private void SelectNextTrack()
    {
        StackPanel stackPanel = TracksGrid.Children[0] as StackPanel;
        TrackControl currentTrack = stackPanel.Children.Count == 0 ? null : stackPanel.Children[0] as TrackControl;
        TrackControl nextTrack;

        for (int i = 0; i < TracksGrid.Children.Count; i++)
        {

        }
    }
    */

    private void SelectNextTrack()
    {
        int index = trackControls.IndexOf(selectedTrack);

        int lastFocusedCarIndex = selectedTrack.Unselect();
        selectedTrack = trackControls[(index + 1) % trackControls.Count];
        selectedTrack.Select(lastFocusedCarIndex);
    }

    private void SelectPreviousTrack()
    {
        int index = trackControls.IndexOf(selectedTrack);

        int lastFocusedCarIndex = selectedTrack.Unselect();
        selectedTrack = trackControls[(index - 1 + trackControls.Count) % trackControls.Count];
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
        foreach (TrackControl trackControl in trackControls)
        {
            trackControl.ZoomIn();
        }

        TracksBorder.Width = trackControls[0].Width + 18;
        foreach (TrackControl child in trackControls)
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
        foreach (TrackControl trackControl in trackControls)
        {
            trackControl.ZoomOut();
        }

        TracksBorder.Width = trackControls[0].Width + 18;
        foreach (TrackControl child in trackControls)
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
        int currentIndex = trackControls.IndexOf(startTrack);

        var transform = startTrack.TransformToVisual(TracksGrid);

        var position = transform.Value.Transform(point);

        foreach (TrackControl trackControl in trackControls)
        {
            if (HitTest(trackControl, position)) return trackControl;
        }

        return null;
    }

    public bool HitTest(TrackControl trackControl, Point point)
    {
        var transform = trackControl.TransformToVisual(TracksGrid);

        var topLeft = transform.Value.Transform(new Point(0, 0));
        var bottomRight = transform.Value.Transform(new Point(trackControl.Bounds.Width, trackControl.Bounds.Height));

        if (point.X > topLeft.X && point.X < bottomRight.X)
        {
            if (point.Y < bottomRight.Y && point.Y > topLeft.Y)
            {
                return true;
            }
        }
        return false;
    }
}