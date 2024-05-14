using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.CustomControls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.Generic;
using AvaloniaApplication2.Models;
using System;

namespace AvaloniaApplication2;

public partial class StationStateWindow : Window
{
    public Station Station { get; }
    public StationControl StaionControl;

    public StationStateWindow(Station station, MainWindowViewModel dataContext)
    {
        DataContext = dataContext;
        Station = station;

        StationControl stationControl = new StationControl(this, Station);
        StaionControl = stationControl;

        InitializeComponent();

        StationPanel.Children.Add(stationControl);

        Title = Station.StationName;

        HotKeyManager.SetHotKey(StaionControl, new KeyGesture(Key.Tab));

        AddHandler(KeyDownEvent, StationWindow_KeyDown, RoutingStrategies.Tunnel);
    }

    protected void StationWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        StaionControl.StationControl_KeyDown(this, e);
    }

    private void StationWindow_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        StaionControl.Width = Width;
        StaionControl.Height = Height;
    }

    private void StationWindow_Activated(object? sender, EventArgs e)
    {
        ((MainWindowViewModel)DataContext).SelectedStation = StaionControl.Station;
    }

    public void MoveCars()
    {
        MoveCarsDialogWindow moveCarsDialogWindow = new MoveCarsDialogWindow();

        moveCarsDialogWindow.ShowDialog(this);
    }
}