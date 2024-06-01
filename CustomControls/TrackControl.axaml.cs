using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Views;
using iText.StyledXmlParser.Jsoup.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Track = AvaloniaApplication2.Models.Track;

namespace AvaloniaApplication2.CustomControls;

public partial class TrackControl : UserControl
{
    private readonly int[] CAR_WIDTHS = { 6, 7, 8, 9, 10, 11, 12 };
    private readonly int[] TRACK_CONTROL_HEIGHS = { 41, 44, 46, 48, 51, 53, 56};
    private static readonly int[] FONT_SIZES = { 11, 12, 12, 12, 13, 13, 14 };

    private MainWindowViewModel viewModel;
    private StationControl ParentControl;
    public int ZoomLevel = 1;

    private bool isFirstClick = true;
    private bool isDoubleClick = false;
    private DateTime firstClickTime;
    private const int DoubleClickThreshold = 150;

    private static bool _isPointerPressed = false;
    private static bool _isCarsDrugging = false;
    private Point _pressedPoint;
    private Control? _pressedControl;

    public bool IsSelected { get { return (DataContext as MainWindowViewModel).SelectedTrack == Track; } }
    public Track Track { get; }
    private List<CarControl> focusedCars;
    public static int TURNOUT_WIDTH = 40;

    public TrackControl()
    {
        InitializeComponent();
    }

    public TrackControl(Track track, StationControl parentStation)
    {
        focusedCars = new List<CarControl>();

        Track = track;
        ParentControl = parentStation;

        DataContext = parentStation.DataContext as MainWindowViewModel;
        viewModel = DataContext as MainWindowViewModel;

        InitializeComponent();

        AddHandler(KeyDownEvent, TrackControl_KeyDown, RoutingStrategies.Tunnel);

        AddHandler(PointerPressedEvent, TrackControl_PointerPressed, RoutingStrategies.Bubble);

        AddHandler(PointerMovedEvent, TrackControl_PointerMoved);

        AddHandler(PointerReleasedEvent, TrackControl_PointerReleased, RoutingStrategies.Bubble);

        TrackNumberTextBlock.Text = track.TrackNumber.ToString();
        CarsCountTextBlock.Text = track.Cars.Count.ToString();

        AddCarsToTrackGrid();
        UpdateSizes();
    }

    private void AddCarsToTrackGrid()
    {
        var width = TURNOUT_WIDTH * 2 + CAR_WIDTHS[ZoomLevel] * Track.Capacity;
        TrackWrapper.Width = width;
        BottomLine.EndPoint = new Point(width - 20, 1);

        for (var i = 0; i < Track.Capacity; i++)
        {
            ColumnDefinition cd = new ColumnDefinition
            {
                Width = new GridLength(CAR_WIDTHS[ZoomLevel], GridUnitType.Pixel)
            };

            TrackGrid.ColumnDefinitions.Add(cd);

            if (i < Track.Cars.Count)
            {
                TrackGrid.Children.Add(new CarControl(Track.GetCar(i + 1), this)
                {
                    [Grid.RowProperty] = 0,
                    [Grid.ColumnProperty] = i,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 2)
                });
            }
            else
            {
                TrackGrid.Children.Add(new CarControl(this)
                {
                    [Grid.RowProperty] = 0,
                    [Grid.ColumnProperty] = i,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 2)
                });
            }
        }
    }

    public void TrackControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        {
            firstClickTime = DateTime.Now;

            if (isFirstClick)
            {
                // Single Click
                _isPointerPressed = true;
                _pressedPoint = e.GetCurrentPoint(TrackWrapper).Position;
                _pressedControl = (sender is TrackControl) ? sender as TrackControl : sender as CarControl;
                SelectingRect[Canvas.TopProperty] = _pressedPoint.Y;
                SelectingRect[Canvas.LeftProperty] = _pressedPoint.X; _isPointerPressed = true;
            }
            else
            {
                // Double Click
                isDoubleClick = true;
                isFirstClick = true;

                if (sender is CarControl carControl)
                {
                    if (carControl.IsCarFocused)
                    {
                        _isCarsDrugging = true;
                        viewModel.SelectedStation = ParentControl.Station;
                        viewModel.SelectedTrack = Track;
                        foreach (CarControl focusedCar in focusedCars)
                        {
                            focusedCar.IsDrugging = true;
                        }
                    }
                }
            }

            ParentControl.StationControl_PointerPressed(this, e);
        }
    }

    private void TrackControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isPointerPressed)
        {
            var currentPoint = e.GetCurrentPoint(TrackWrapper).Position;

            double x = Math.Min(currentPoint.X, _pressedPoint.X);
            double y = Math.Min(currentPoint.Y, _pressedPoint.Y);

            double width = Math.Abs(currentPoint.X - _pressedPoint.X);
            double height = Math.Abs(currentPoint.Y - _pressedPoint.Y);

            SelectingRect.Width = width;
            SelectingRect.Height = height;

            Canvas.SetLeft(SelectingRect, x);
            Canvas.SetTop(SelectingRect, y);
        }
    }

    public void TrackControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            var timeSinceFirstClick = DateTime.Now - firstClickTime;

            if (timeSinceFirstClick.TotalMilliseconds > DoubleClickThreshold)
            {
                if (isDoubleClick)
                {
                    foreach (CarControl focusedCar in focusedCars)
                    {
                        if (!focusedCar.IsSelected && viewModel.IsOperator)
                        {
                            focusedCar.IsSelected = true;
                        }                            
                        focusedCar.IsDrugging = false;
                    }

                    isDoubleClick = false;

                    if (viewModel.IsOperator)
                    {
                        var point = e.GetCurrentPoint(TrackWrapper).Position;
                        if (point.Y < 0 || point.Y > Height)
                        {
                            TrackControl destTrack = ParentControl.GetTrackControlByPoint(this, point);

                            if (destTrack != null)
                            {
                                viewModel.MainWindow.OpenMovingCarsWindow(ParentControl.Station, destTrack.Track);
                            }
                        }
                    }

                }
                else
                {
                    if (_pressedControl != null && _pressedControl == sender)
                    {
                        UnfocusAllCars();

                        var point = e.GetCurrentPoint(TrackWrapper).Position;

                        double left = Math.Min(point.X, _pressedPoint.X);
                        double top = Math.Min(point.Y, _pressedPoint.Y);

                        var rect = new Rect(left, top, SelectingRect.Width, SelectingRect.Height);

                        var transform = TrackGrid.TransformToVisual(TrackWrapper);

                        foreach (CarControl carControl in TrackGrid.Children)
                        {
                            if (!carControl.IsEmpty)
                            {
                                var carPosition = transform.Value.Transform(new Point(carControl.Bounds.X, carControl.Bounds.Y));
                                var carRect = new Rect(carPosition, carControl.Bounds.Size);

                                if (rect.Intersects(carRect))
                                {
                                    FocusCarControl(carControl);
                                }
                            }

                        }
                    }
                }                
            }
            else
            {

                // Not druging
                if (isDoubleClick)
                {
                    foreach (CarControl focusedCar in focusedCars)
                    {
                        focusedCar.IsDrugging = false;
                    }

                    isDoubleClick = false;

                    if (sender is CarControl carControl)
                    {
                        UnfocusAllCars();
                        FocusNthCar(carControl.Car.SerialNumber - 1);
                        SelectAllCars();
                    }
                }
                else
                {
                    isFirstClick = false;

                    Task.Delay(DoubleClickThreshold).ContinueWith(t =>
                    {
                        if (!isFirstClick)
                        {
                            if (sender is CarControl carControl)
                            {
                                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    UnfocusAllCars();
                                    FocusNthCar(carControl.Car.SerialNumber - 1);
                                });
                            }
                        }
                        isFirstClick = true;
                    });
                }
            }
            
            _isPointerPressed = false;
            _isCarsDrugging = false;
            SelectingRect.Width = 0;
            SelectingRect.Height = 0;

            _pressedControl = null;

            if (!IsSelected)
            {
                Select();
            }

            e.Handled = true;
        }
    }

    public void Select(int focusedCarNumber = -1)
    {
        TrackBorderWrapper.BorderThickness = new Thickness(1);
        if (focusedCarNumber != -1) FocusNthCar(focusedCarNumber);
        (DataContext as MainWindowViewModel).SelectedTrack = Track;
        SetMenuItemsEnabling();
    }

    public int Unselect()
    {
        TrackBorderWrapper.BorderThickness = new Thickness(0);
        int lastFocusedCarIndex = (focusedCars.Count == 0) ? 0 : focusedCars[focusedCars.Count - 1].Car.SerialNumber - 1;
        UnfocusAllCars();
        if ((DataContext as MainWindowViewModel).SelectedTrack == Track)
        {
            (DataContext as MainWindowViewModel).SelectedTrack = null;
        }
        SetMenuItemsEnabling();

        return lastFocusedCarIndex;
    }


    public void UpdateTrack()
    {
        int selectedCount = 0;

        foreach (Car car in Track.Cars)
        {
            if (car.IsSelected) selectedCount++;
            ((CarControl)TrackGrid.Children[car.SerialNumber - 1]).UpdateCar(this, car);
        }
        for (int i = Track.Cars.Count; i < TrackGrid.Children.Count; i++)
        {
            ((CarControl)TrackGrid.Children[i]).UpdateCar(this);
        }

        focusedCars.Clear();
        foreach (CarControl carControl in TrackGrid.Children)
         {
            if (carControl.IsCarFocused)
            {
                focusedCars.Add(carControl);
            }
        }

        SelectedCarsCountTextBlock.Text = selectedCount.ToString();
        CarsCountTextBlock.Text = Track.Cars.Count.ToString();
    }

    public void TrackControl_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Tab:
                if (focusedCars.Count == 0)
                {
                    FocusNthCar(0);
                }
                else
                {
                    if (e.KeyModifiers == KeyModifiers.Shift)
                    {
                        FocusPreviousCar();
                    }
                    else
                    {
                        if (e.KeyModifiers == KeyModifiers.None)
                        {
                            FocusNextCar();
                        }
                    }
                }
                break;

            case Key.Enter:
                SelectAllCars();
                break;

            case Key.Right:
                if (e.KeyModifiers == KeyModifiers.Shift)
                {
                    FocusNextCar(true);
                }
                else
                {
                    if (e.KeyModifiers == KeyModifiers.None)
                    {
                        FocusNextCar();
                    }
                }
                break;

            case Key.Left:
                if (e.KeyModifiers == KeyModifiers.Shift)
                {
                    FocusPreviousCar(true);
                }
                else
                {
                    if (e.KeyModifiers == KeyModifiers.None)
                    {
                        FocusPreviousCar();
                    }
                }
                break;

            case Key.F3:
                viewModel.MainWindow.OpenNewComingWindow();
                break;

            case Key.F4:
                viewModel.MainWindow.OpenTrackEditWindow();
                break;

            case Key.F6:
                viewModel.MainWindow.OpenMovingCarsWindow();
                break;

            case Key.S:
                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    viewModel.MainWindow.SaveChanges();
                }
                break;
        }
    }

    private void FocusCarControl(CarControl carControl)
    {
        focusedCars.Add(carControl);
        carControl.IsCarFocused = true;
        ParentControl.LastFocusedCar = carControl.Car;
    }

    private void FocusNthCar(int n)
    {
        if (Track.Cars.Count > 0)
        {
            CarControl focusedCar = (n < Track.Cars.Count) ? (CarControl)TrackGrid.Children[n] : (CarControl)TrackGrid.Children[Track.Cars.Count - 1];
            FocusCarControl(focusedCar);
        }
    }

    private void UnfocusAllCars()
    {
        foreach (CarControl carControl in focusedCars)
        {
            carControl.IsCarFocused = false;
        }
        focusedCars.Clear();
        ParentControl.LastFocusedCar = null;
    }

    private void FocusNextCar(bool savePreviusFocused = false)
    {
        if (Track.Cars.Count != 0)
        {
            if (focusedCars.Count == 0)
            {
                FocusNthCar(0);
            }
            else
            {
                int nextIndex;
                int curreIndex = focusedCars[focusedCars.Count - 1].Car.SerialNumber - 1;

                if (focusedCars.Count > 1 && focusedCars[1].Car.SerialNumber < focusedCars[0].Car.SerialNumber)
                {
                    // Справо налево                
                    if (!savePreviusFocused)
                    {
                        nextIndex = focusedCars[0].Car.SerialNumber % Track.Cars.Count;
                        UnfocusAllCars();
                        FocusNthCar(nextIndex);
                    }
                    else
                    {
                        CarControl currentCar = (CarControl)TrackGrid.Children[curreIndex];
                        currentCar.IsCarFocused = false;
                        focusedCars.Remove(currentCar);
                        ParentControl.LastFocusedCar = focusedCars[focusedCars.Count - 1].Car;
                    }
                }
                else
                {
                    // Слева направо
                    nextIndex = focusedCars[focusedCars.Count - 1].Car.SerialNumber % Track.Cars.Count;

                    CarControl nextCar = TrackGrid.Children[nextIndex] as CarControl;
                    if (!focusedCars.Contains(nextCar))
                    {
                        if (!savePreviusFocused)
                        {
                            UnfocusAllCars();
                        }
                        FocusNthCar(nextIndex);
                    }
                }
            }
        }
    }

    private void FocusPreviousCar(bool savePreviusFocused = false)
    {
        if (Track.Cars.Count != 0)
        {
            if (focusedCars.Count == 0)
            {
                FocusNthCar(Track.Cars.Count - 1);
            }
            else
            {
                int nextIndex;
                int curreIndex = focusedCars[focusedCars.Count - 1].Car.SerialNumber - 1;

                if (focusedCars.Count > 1 && focusedCars[0].Car.SerialNumber < focusedCars[1].Car.SerialNumber)
                {
                    // Слева направо
                    if (!savePreviusFocused)
                    {
                        nextIndex = (focusedCars[0].Car.SerialNumber - 2 + Track.Cars.Count) % Track.Cars.Count;
                        UnfocusAllCars();
                        FocusNthCar(nextIndex);
                    }
                    else
                    {
                        CarControl currentCar = (CarControl)TrackGrid.Children[curreIndex];
                        currentCar.IsCarFocused = false;
                        focusedCars.Remove(currentCar);
                        ParentControl.LastFocusedCar = focusedCars[focusedCars.Count - 1].Car;
                    }
                }
                else
                {
                    // Справо налево
                    nextIndex = (focusedCars[focusedCars.Count - 1].Car.SerialNumber - 2 + Track.Cars.Count) % Track.Cars.Count;
                    CarControl nextCar = TrackGrid.Children[nextIndex] as CarControl;
                    if (!focusedCars.Contains(nextCar))
                    {
                        if (!savePreviusFocused)
                        {
                            UnfocusAllCars();
                        }
                        FocusNthCar(nextIndex);
                    }
                }
            }
        }
    }


    private void SelectAllCars()
    {
        foreach (CarControl carControl in focusedCars)
        {
            carControl.IsSelected = !carControl.IsSelected;
        }

        int selectedCount = 0;
        foreach (CarControl carControl in TrackGrid.Children)
        {
            if (carControl.IsSelected) selectedCount++;
        }
        SelectedCarsCountTextBlock.Text = selectedCount.ToString();
    }

    private void TrackContextMenu_Opened(object? sender, RoutedEventArgs e)
    {
        SetMenuItemsEnabling();
    }

    private void NewComingMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        (DataContext as MainWindowViewModel).MainWindow.OpenNewComingWindow();
    }

    private void TrackEditMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        (DataContext as MainWindowViewModel).MainWindow.OpenTrackEditWindow();
    }

    private void MoveCarsMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.MainWindow.OpenMovingCarsWindow();
    }

    private void SaveMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        (DataContext as MainWindowViewModel).MainWindow.SaveChanges();
    }

    private void FieldSheetMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        (DataContext as MainWindowViewModel).MainWindow.ShowFieldSheet();
    }

    public void SetMenuItemsEnabling()
    {
        if (viewModel.SelectedStation == ParentControl.Station && viewModel.IsOperator)
        {
            NewComingMenuItem.IsEnabled = viewModel.IsOperator;
        }
        else
        {
            NewComingMenuItem.IsEnabled = false;
        }
        if (viewModel.SelectedTrack != null)
        {
            SaveMenuItem.IsEnabled = viewModel.IsOperator;
            TrackEditMenuItem.IsEnabled = viewModel.IsOperator;
            MoveCarsMenuItem.IsEnabled = viewModel.IsOperator;
            FieldSheetMenuItem.IsEnabled = true;
        }
        else
        {
            SaveMenuItem.IsEnabled = false;
            TrackEditMenuItem.IsEnabled = false;
            MoveCarsMenuItem.IsEnabled = false;
            FieldSheetMenuItem.IsEnabled = false;
        }
    }

    public void ZoomIn()
    {
        if (ZoomLevel != CAR_WIDTHS.Length - 1)
        {
            ZoomLevel++;
            
            UpdateSizes();
        }
    }

    public void ZoomOut()
    {
        if (ZoomLevel != 0)
        {
            ZoomLevel--;

            UpdateSizes();
        }
    }

    private void UpdateSizes()
    {
        var width = TURNOUT_WIDTH * 2 + CAR_WIDTHS[ZoomLevel] * Track.Capacity;
        TrackWrapper.Width = width;
        BottomLine.EndPoint = new Point(width - 20, 1);

        Width = width;
        Height = TRACK_CONTROL_HEIGHS[ZoomLevel];

        TrackGrid.Height = CAR_WIDTHS[ZoomLevel] * 2 + 8;
        LeftSwitchBorder.Height = TrackGrid.Height;
        RightSwitchBorder.Height = TrackGrid.Height;
        TrackImageTop.Height = TrackGrid.Height;

        TrackNumberSuffixTextBlock.FontSize = FONT_SIZES[ZoomLevel];
        TrackNumberTextBlock.FontSize = FONT_SIZES[ZoomLevel];
        SelectedCarsCountSuffixTextBlock.FontSize = FONT_SIZES[ZoomLevel];
        SelectedCarsCountTextBlock.FontSize = FONT_SIZES[ZoomLevel];
        CarsCountSuffixTextBlock.FontSize = FONT_SIZES[ZoomLevel];
        CarsCountTextBlock.FontSize = FONT_SIZES[ZoomLevel];

        foreach (ColumnDefinition columnDefinition in TrackGrid.ColumnDefinitions)
        {
            columnDefinition.Width = new GridLength(CAR_WIDTHS[ZoomLevel], GridUnitType.Pixel);
        }

        foreach (CarControl carControl in TrackGrid.Children)
        {
            carControl.UpdateCarImageSize();
        }
    }
}