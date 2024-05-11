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
    private bool IsSelected { get; set; } = false;
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

        this.AddHandler(PointerPressedEvent, TrackControl_PointerPressed_Tunnel, RoutingStrategies.Tunnel);

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
                TrackGrid.Children.Add(new CarControl(track.Cars[i])
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

    private void TrackControl_PointerPressed_Tunnel(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (!IsSelected)
        {
            e.Handled = true;
        }
        Select();
        if (DataContext != null)
        {
            parent.SelectTrack(this);
            Debug.WriteLine("Selected track: " + Track.ToString());
        }
    }

    public void Select()
    {
        IsSelected = true;
        TrackBorderWrapper.BorderThickness = new Thickness(1);
    }

    public void Unselect()
    {
        TrackBorderWrapper.BorderThickness = new Thickness(0);
        foreach (CarControl carControl in focusedCars)
        {
            carControl.IsFocused = false;
        }
        focusedCars.Clear();
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
        if (e.Key == Key.Tab || e.Key == Key.Right)
        {
            if (focusedCars.Count == 0) 
            {
                CarControl firstCar = ((CarControl)TrackGrid.Children[0]);
                if (!firstCar.IsEmpty)
                {
                    firstCar.IsFocused = true;
                    focusedCars.Add(firstCar);
                }
            }
            else
            {
                int nextIndex = focusedCars[0].Car.SerialNumber;
                CarControl nextCar = ((CarControl)TrackGrid.Children[nextIndex]);
                if (nextCar.IsEmpty)
                {
                    nextCar = (CarControl)TrackGrid.Children[0];
                }
                focusedCars[0].IsFocused = false;
                focusedCars.Clear();
                focusedCars.Add(nextCar);
                nextCar.IsFocused = true;
            }
            
        }
        if (e.Key == Key.Enter)
        {
            foreach (CarControl carControl in TrackGrid.Children)
            {
                if (carControl.IsFocused) carControl.IsSelected = !carControl.IsSelected;
            }
        }
    }
}