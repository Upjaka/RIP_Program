using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;

namespace AvaloniaApplication2.CustomControls;

public partial class CarControl : UserControl
{
    public bool IsEmpty { get; }
    public Car Car { get; set; }
    public bool IsSelected 
    {
        get { return Car.IsSelected; }
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
        IsEmpty = true;
    }

    public CarControl(Car car)
    {
        Car = car;
        IsEmpty = false;
        InitializeComponent();

        this.AddHandler(PointerPressedEvent, CarCotrol_PointerPressed_Tunnel, RoutingStrategies.Tunnel);

        AssignStyles();
    }

    public void UpdateCar()
    {
        CarRectangle.Classes.RemoveAll(["Fixed", "Loaded", "Empty"]);
        CarRectangle.Classes.Add("Empty");

        AssignStyles();
    }

    private void CarCotrol_PointerPressed_Tunnel(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (!IsFocused)
        {
            IsFocused = true;
        }
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
    }

    public void UnselectCar()
    {
        Car.IsSelected = false;

        CarBorderWrapper.Classes.Remove("Selected");
    }
}