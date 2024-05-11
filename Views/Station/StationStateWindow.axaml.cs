using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.CustomControls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaApplication2;

public partial class StationStateWindow : Window
{
    private StationControl _staionControl;
    public StationStateWindow(StationControl stationControl, MainWindowViewModel dataContext)
    {
        _staionControl = stationControl;
        InitializeComponent();
        DataContext = dataContext;
        StationPanel.Children.Add(stationControl);

        Title = stationControl.station.StationName;

        HotKeyManager.SetHotKey(_staionControl, new KeyGesture(Key.Tab));

        this.AddHandler(KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);
        //StationBorderWrapper.AddHandler(KeyDownEvent, Border_KeyDown, RoutingStrategies.Tunnel);
    }

    protected void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        _staionControl.StationControl_KeyDown(this, e);
    }

    private void Window_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        _staionControl.Width = Width;
        _staionControl.Height = Height;
    }
}