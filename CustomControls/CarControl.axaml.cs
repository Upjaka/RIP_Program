using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;

namespace AvaloniaApplication2.CustomControls;

public partial class CarControl : UserControl
{
    public CarControl()
    {
        InitializeComponent();
    }

    public CarControl(Car car)
    {
        InitializeComponent();
        if (car.IsFixed)
        {
            CarRectangle.Classes.Add("Fixed");
            CarRectangle.Classes.Remove("Empty");
        }
        if (car.IsLoaded)
        {
            CarRectangle.Classes.Add("Loaded");
            CarRectangle.Classes.Remove("Empty");
        }
    }
}