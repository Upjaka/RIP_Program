using Avalonia;
using Avalonia.Controls;
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

namespace AvaloniaApplication2.CustomControls;

public partial class TrackControl : UserControl
{
    private MainWindowViewModel viewModel;
    private Window ParentWindow;
    private StationControl ParentControl;

    private static bool _isDrugging = false;
    private Point _pressedPoint;

    public bool IsSelected { get { return (DataContext as MainWindowViewModel).SelectedTrack == Track; } }
    public Track Track { get; }
    private List<CarControl> focusedCars;
    public static int CAR_WIDTH = 8;
    public static int TURNOUT_WIDTH = 40;

    public TrackControl()
    {
        InitializeComponent();
    }

    public TrackControl(Track track, StationControl parentStation)
    {
        focusedCars = new List<CarControl>();

        Track = track;
        ParentWindow = parentStation.ParentWindow;
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

        var width = TURNOUT_WIDTH * 2 + CAR_WIDTH * track.Capacity;
        TrackWrapper.Width = width;
        BottomLine.EndPoint = new Point(width - 20, 1);

        for (var i = 0; i < track.Capacity; i++)
        {
            ColumnDefinition cd = new ColumnDefinition
            {
                Width = new GridLength(CAR_WIDTH, GridUnitType.Pixel)
            };

            TrackGrid.ColumnDefinitions.Add(cd);

            if (i < track.Cars.Count)
            {
                TrackGrid.Children.Add(new CarControl(track.GetCar(i + 1), this)
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
            _isDrugging = true;
            _pressedPoint = e.GetCurrentPoint(TrackWrapper).Position;
            SelectingRect[Canvas.TopProperty] = _pressedPoint.Y;
            SelectingRect[Canvas.LeftProperty] = _pressedPoint.X;

            if (!IsSelected)
            {
                Select();
            }
            ParentControl.StationControl_PointerPressed(this, e);
        }
    }

    private void TrackControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDrugging)
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

            _isDrugging = false;
            SelectingRect.Width = 0;
            SelectingRect.Height = 0;
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
        MoveCarsMenuItem.IsEnabled = (DataContext as MainWindowViewModel).SelectedTrack != null && (DataContext as MainWindowViewModel).IsOperator;
        FieldSheetMenuItem.IsEnabled = (DataContext as MainWindowViewModel).SelectedTrack != null && (DataContext as MainWindowViewModel).IsOperator;
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
        if (viewModel.SelectedTrack != null && viewModel.IsOperator)
        {
            SaveMenuItem.IsEnabled = true;
            NewComingMenuItem.IsEnabled = true;
            TrackEditMenuItem.IsEnabled = true;
            MoveCarsMenuItem.IsEnabled = true;
            FieldSheetMenuItem.IsEnabled = true;
        }
        else
        {
            SaveMenuItem.IsEnabled = false;
            NewComingMenuItem.IsEnabled = false;
            TrackEditMenuItem.IsEnabled = false;
            MoveCarsMenuItem.IsEnabled = false;
            FieldSheetMenuItem.IsEnabled = false;
        }
    }
}