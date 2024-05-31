using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using iText.Layout.Borders;
using System;
using System.Diagnostics;

namespace AvaloniaApplication2.CustomControls;

public partial class CarControl : UserControl
{
    private readonly int[] CAR_WIDTHS = { 6, 7, 8, 9, 10, 11, 12 };

    private int ZoomLevel = 1;
    private TrackControl ParentControl;
    private MainWindowViewModel viewModel;
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
    public bool IsCarFocused
    {
        get { return _isFocused; }
        set 
        {
            if (_isFocused != value)
            {
                _isFocused = value;
                if (IsCarFocused)
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
        DataContext = parent.DataContext;
        viewModel = DataContext as MainWindowViewModel;
        InitializeComponent();
        CarImage.Width = CAR_WIDTHS[ZoomLevel];
        CarImage.Height = CAR_WIDTHS[ZoomLevel] * 2;
        CarWrapper.Width = CarImage.Width + 2;
        CarWrapper.Height = CarImage.Height + 2;
        CarImage.IsVisible = false;
    }

    public CarControl(Car car, TrackControl parent)
    {
        ParentControl = parent;
        DataContext = parent.DataContext;
        viewModel = DataContext as MainWindowViewModel;
        Car = car;
        InitializeComponent();

        UpdateCarImageSize();
        CarImage.Source = CarCreator.CreateImage(Car.IsFixed, Car.IsLoaded);

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
                CarImage.IsVisible = true;
                AddHandler(PointerPressedEvent, CarControl_PointerPressed, RoutingStrategies.Bubble);
                AddHandler(PointerReleasedEvent, CarControl_PointerReleased, RoutingStrategies.Bubble);
            }
            Car = car;
            AssignStyles();
        }
        else
        {
            Car = null;
            CarImage.IsVisible = false;
            RemoveStyles();
        }      
    }

    private void RemoveStyles()
    {
        IsCarFocused = false;
        CarImage.Classes.RemoveAll(["Fixed", "Loaded"]);
        CarBorderWrapper.Classes.Remove("Selected");
    }

    private void AssignStyles()
    {
        if (Car != null)
            CarImage.Source = CarCreator.CreateImage(Car.IsFixed, Car.IsLoaded);

        RemoveStyles();

        if (Car.IsFixed)
        {
            CarImage.Classes.Add("Fixed");
        }
        if (Car.IsLoaded)
        {
            CarImage.Classes.Add("Loaded");
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

    private void CarControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ParentControl.TrackControl_PointerPressed(this, e);
    }

    private void CarControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ParentControl.TrackControl_PointerReleased(this, e);
    }

    public void UpdateCarImageSize()
    {
        CarImage.Width = CAR_WIDTHS[ParentControl.ZoomLevel];
        CarImage.Height = CAR_WIDTHS[ParentControl.ZoomLevel] * 2;
        CarWrapper.Width = CarImage.Width + 2;
        CarWrapper.Height = CarImage.Height + 2;
        if (Car !=  null)
        {
            CarImage.Source = CarCreator.CreateImage(Car.IsFixed, Car.IsLoaded);
        }        
    }
}