using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;

namespace AvaloniaApplication2.CustomControls;

public partial class CarControl : UserControl
{
    private TrackControl ParentControl;
    public bool IsEmpty { get { return Car == null; } }
    public Car Car { get; set; }
    public bool IsSelected 
    {
        get { return (Car == null) ? false : Car.IsSelected; }
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

    public CarControl()
    {
        InitializeComponent();
        CarRectangle.IsVisible = false;
    }

    public CarControl(Car car, TrackControl parent)
    {
        ParentControl = parent;
        Car = car;
        InitializeComponent();

        AddHandler(PointerPressedEvent, CarCotrol_PointerPressed, RoutingStrategies.Bubble);

        AssignStyles();
    }

    public void UpdateCar(Car car)
    {
        if (car == null)
        {
            Car = null;
            CarRectangle.IsVisible = false;
            CarBorderWrapper.Classes.RemoveAll(["Selected", "Focused"]);
            CarRectangle.Classes.Add("Empty");
        }
        else
        {
            if (Car != car)
            {
                Car = car;
                CarBorderWrapper.Classes.Remove("Focused");
            }

            CarRectangle.IsVisible = !IsEmpty;

            if (Car != null)
            {
                CarRectangle.Classes.RemoveAll(["Fixed", "Loaded", "Empty"]);
                CarBorderWrapper.Classes.Remove("Selected");
                CarRectangle.Classes.Add("Empty");

                AssignStyles();
            }
        }
    }

    private void CarCotrol_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        ParentControl.TrackControl_PointerPressed(this, e);
    }

    private void AssignStyles()
    {
        if (Car.IsFixed)
        {
            CarRectangle.Classes.Add("Fixed");
            CarRectangle.Classes.Remove("Empty");
        }
        if (Car.IsLoaded)
        {
            CarRectangle.Classes.Add("Loaded");
            CarRectangle.Classes.Remove("Empty");
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
}