using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Views;
using System.Collections.Generic;
using System.Diagnostics;

namespace AvaloniaApplication2.CustomControls;

public partial class TrackControl : UserControl
{
    private StationStateWindow ParentWindow;
    private StationControl ParentControl;

    public bool IsSelected { get; set; } = false;
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

        DataContext = ParentWindow.DataContext as MainWindowViewModel;

        InitializeComponent();

        AddHandler(KeyDownEvent, TrackControl_KeyDown, RoutingStrategies.Tunnel);

        AddHandler(PointerPressedEvent, TrackControl_PointerPressed, RoutingStrategies.Bubble);

        TrackNumberTextBlock.Text = track.TrackNumber.ToString();
        CarsCountTextBlock.Text = track.Cars.Count.ToString();

        var width = TURNOUT_WIDTH * 2 + CAR_WIDTH * track.Capacity;
        Wrapper.Width = width;
        BottomLine.EndPoint = new Point(width - 20, 1);

        for (var i=0; i<track.Capacity; i++)
        {
            ColumnDefinition cd = new ColumnDefinition { 
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
                TrackGrid.Children.Add(new CarControl()
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
            if (sender is CarControl)
            {
                if (!IsSelected)
                {
                    Select(((CarControl)sender).Car.SerialNumber - 1);
                }
                else
                {
                    UnfocusAllCars();
                    FocusNthCar(((CarControl)sender).Car.SerialNumber - 1);
                }
            }
            else
            {
                if (!IsSelected)
                {
                    Select();
                }
            }
            ParentControl.StationControl_PointerPressed(this, e);
        }
    }

    public void Select(int focusedCarNumber = 0)
    {
        IsSelected = true;
        TrackBorderWrapper.BorderThickness = new Thickness(1);
        FocusNthCar(focusedCarNumber);
        (DataContext as MainWindowViewModel).SelectedTrack = Track;
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
            
        return lastFocusedCarIndex;
    }


    public void UpdateTrack()
    {
        int selectedCount = 0;

        foreach (Car car in Track.Cars)
        {
            if (car.IsSelected) selectedCount++;
            ((CarControl)TrackGrid.Children[car.SerialNumber - 1]).UpdateCar(car);
        }
        for (int i = Track.Cars.Count; i < TrackGrid.Children.Count; i++)
        {
            ((CarControl)TrackGrid.Children[i]).UpdateCar(null);
        }
        focusedCars.Clear();
        foreach (CarControl carControl in TrackGrid.Children)
        {
            if (carControl.IsFocused)
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
        }
    }

    private void FocusNthCar(int n)
    {
        if (Track.Cars.Count > 0)
        {
            CarControl focusedCar = (n < Track.Cars.Count) ? (CarControl)TrackGrid.Children[n] : (CarControl)TrackGrid.Children[Track.Cars.Count - 1];
            focusedCar.IsFocused = true;
            focusedCars.Add(focusedCar);
        }
    }

    private void UnfocusAllCars()
    {
        foreach (CarControl carControl in focusedCars)
        {
            carControl.IsFocused = false;
        }
        focusedCars.Clear();
    }

    private void FocusNextCar(bool savePreviusFocused = false)
    {
        if (Track.Cars.Count != 0)
        {
            int nextIndex = focusedCars[focusedCars.Count - 1].Car.SerialNumber % Track.Cars.Count;
            if (focusedCars.Count > 1 && focusedCars[0].Car.SerialNumber > focusedCars[1].Car.SerialNumber)
            {
                nextIndex = focusedCars[0].Car.SerialNumber % Track.Cars.Count;
            }
            CarControl nextCar = ((CarControl)TrackGrid.Children[nextIndex]);
            if (!savePreviusFocused)
            {
                UnfocusAllCars();
                FocusNthCar(nextIndex);
            }
            else
            {
                if (nextCar.IsFocused)
                {
                    CarControl currentCar = (CarControl)TrackGrid.Children[(nextIndex - 1 + Track.Cars.Count) % Track.Cars.Count];
                    currentCar.IsFocused = false;
                    focusedCars.Remove(currentCar);
                }
                else
                {
                    FocusNthCar(nextIndex);
                }
            }
        }        
    }

    private void FocusPreviousCar(bool savePreviusFocused = false)
    {
        if (Track.Cars.Count != 0)
        {
            int nextIndex = (focusedCars[focusedCars.Count - 1].Car.SerialNumber - 2 + Track.Cars.Count) % Track.Cars.Count;
            if (focusedCars.Count > 1 && focusedCars[0].Car.SerialNumber < focusedCars[1].Car.SerialNumber)
            {
                nextIndex = (focusedCars[0].Car.SerialNumber - 2 + Track.Cars.Count) % Track.Cars.Count;
            }
            CarControl nextCar = ((CarControl)TrackGrid.Children[nextIndex]);
            if (!savePreviusFocused)
            {
                UnfocusAllCars();
                FocusNthCar(nextIndex);
            }
            else
            {
                if (nextCar.IsFocused)
                {
                    CarControl currentCar = (CarControl)TrackGrid.Children[(nextIndex + 1) % Track.Cars.Count];
                    currentCar.IsFocused = false;
                    focusedCars.Remove(currentCar);
                }
                else
                {
                    FocusNthCar(nextIndex);
                }
            }
        }
    }

    private void SelectAllCars()
    {
        foreach (CarControl carControl in focusedCars)
        {
            carControl.IsSelected = true;
        }

        int selectedCount = 0;
        foreach (CarControl carControl in TrackGrid.Children)
        {
            if (carControl.IsSelected) selectedCount++;
        }
        SelectedCarsCountTextBlock.Text = selectedCount.ToString();
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
        ParentWindow.MoveCars_Clicked();
    }

    private void DevelopTrackMenuItem_Click(object? sender, RoutedEventArgs e)
    {

    }

    private void TrackContextMenu_Opened(object? sender, RoutedEventArgs e)
    {
        MoveCarsMenuItem.IsEnabled = (DataContext as MainWindowViewModel).SelectedTrack != null;
    }
}