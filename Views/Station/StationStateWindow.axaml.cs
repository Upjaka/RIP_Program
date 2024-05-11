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
    public StationControl StaionControl;
    public StationStateWindow(StationControl stationControl, MainWindowViewModel dataContext)
    {
        StaionControl = stationControl;
        InitializeComponent();
        DataContext = dataContext;
        StationPanel.Children.Add(stationControl);

        Title = stationControl.Station.StationName;

        HotKeyManager.SetHotKey(StaionControl, new KeyGesture(Key.Tab));

        this.AddHandler(KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);
        //StationBorderWrapper.AddHandler(KeyDownEvent, Border_KeyDown, RoutingStrategies.Tunnel);
    }

    protected void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        StaionControl.StationControl_KeyDown(this, e);
    }

    private void Window_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        StaionControl.Width = Width;
        StaionControl.Height = Height;
    }

    private void StationWindow_Activated(object? sender, System.EventArgs e)
    {
        ((MainWindowViewModel)DataContext).SelectedStation = StaionControl.Station;
    }
}