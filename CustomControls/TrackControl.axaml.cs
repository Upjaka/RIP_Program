using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;

namespace AvaloniaApplication2.CustomControls;

public partial class TrackControl : UserControl
{
    public bool IsSelected { get; set; } = false;
    public Track Track { get; }
    private List<CarControl> focusedCars;
    private StationControl parent;
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
        parent = parentStation;


        InitializeComponent();

        this.AddHandler(KeyDownEvent, TrackControl_KeyDown, RoutingStrategies.Tunnel);

        this.AddHandler(PointerPressedEvent, TrackControl_PointerPressed, RoutingStrategies.Bubble);

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
                TrackGrid.Children.Add(new CarControl(track.Cars[i], this)
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

    public void TrackControl_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
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
        parent.StationControl_PointerPressed(this, e);
    }

    public void Select(int focusedCarNumber = 0)
    {
        IsSelected = true;
        TrackBorderWrapper.BorderThickness = new Thickness(1);
        FocusNthCar(focusedCarNumber);
    }

    public int Unselect()
    {
        TrackBorderWrapper.BorderThickness = new Thickness(0);
        int lastFocusedCarIndex = (focusedCars.Count == 0) ? 0 : focusedCars[focusedCars.Count - 1].Car.SerialNumber - 1;
        UnfocusAllCars();
        return lastFocusedCarIndex;
    }


    public void UpdateTrack()
    {
        foreach (CarControl carControl in TrackGrid.Children)
        {
            carControl.UpdateCar();
        }
    }

    public void TrackControl_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
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

    private void FocusPreviousCar(bool savePreviusFocused = false)
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

    private void SelectAllCars()
    {
        foreach (CarControl carControl in focusedCars)
        {
            carControl.IsSelected = !carControl.IsSelected;
        }
    }
}