using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;
using iText.Layout.Borders;
using System;
using System.Diagnostics;

namespace AvaloniaApplication2.CustomControls;

public partial class CarControl : UserControl
{

    private TrackControl ParentControl;
    public bool IsEmpty { get { return Car == null; } }
    public Car? Car { get; set; }
    public bool IsSelected 
    {
        get { return (IsEmpty) ? false : Car.IsSelected; }
        set {
            Car.IsSelected = value;
            if (IsSelected)
            {
                CarBorderWrapper.Classes.Add("Selected");
            }
            else
            {
                CarBorderWrapper.Classes.Remove("Selected");
            }
        }
    }
    private bool _isFocused;
    public bool IsFocused
    {
        get { return _isFocused; }
        set 
        {
            if (_isFocused != value)
            {
                _isFocused = value;
                if (IsFocused)
                {
                    CarBorderWrapper.Classes.Add("Focused");
                }
                else
                {
                    CarBorderWrapper.Classes.Remove("Focused");
                }
            }
        }
    }

    public CarControl(TrackControl parent)
    {
        Car = null;
        ParentControl = parent;
        InitializeComponent();
        CarRectangle.IsVisible = false;
    }

    public CarControl(Car car, TrackControl parent)
    {
        ParentControl = parent;
        Car = car;
        InitializeComponent();

        AddHandler(PointerPressedEvent, CarControl_PointerPressed, RoutingStrategies.Bubble);
        AddHandler(PointerReleasedEvent, CarControl_PointerReleased, RoutingStrategies.Bubble);

        AssignStyles();
    }

    public void UpdateCar(TrackControl parent, Car car = null)
    {
        ParentControl = parent;

        if (car != null)
        {
            if (Car == null)
            {
                CarRectangle.IsVisible = true;
                AddHandler(PointerPressedEvent, CarControl_PointerPressed, RoutingStrategies.Bubble);
                AddHandler(PointerReleasedEvent, CarControl_PointerReleased, RoutingStrategies.Bubble);
            }
            Car = car;
            AssignStyles();
        }
        else
        {
            Car = null;
            CarRectangle.IsVisible = false;
            RemoveStyles();
        }      
    }

    private void RemoveStyles()
    {
        IsFocused = false;
        CarRectangle.Classes.RemoveAll(["Fixed", "Loaded"]);
        CarBorderWrapper.Classes.Remove("Selected");
    }

    private void AssignStyles()
    {
        RemoveStyles();

        if (Car.IsFixed)
        {
            CarRectangle.Classes.Add("Fixed");
        }
        if (Car.IsLoaded)
        {
            CarRectangle.Classes.Add("Loaded");
        }
        if (Car.IsSelected)
        {
            CarBorderWrapper.Classes.Add("Selected");
        }
    }

    public void UnselectCar()
    {
        Car.IsSelected = false;

        CarBorderWrapper.Classes.Remove("Selected");
    }

    private void CarControl_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        ParentControl.TrackControl_PointerPressed(this, e);
    }

    private void CarControl_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        ParentControl.TrackControl_PointerReleased(this, e);
    }
}