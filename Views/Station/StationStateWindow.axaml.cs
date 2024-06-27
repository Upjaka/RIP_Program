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
    public StationControl StationControl;

    public StationStateWindow(StationControl stationControl, MainWindowViewModel dataContext)
    {
        DataContext = dataContext;
        Station = stationControl.Station;

        //StationControl stationControl = new StationControl(this, Station);
        StationControl = stationControl;

        InitializeComponent();

        StationPanel.Children.Add(stationControl);

        Title = Station.StationName;

        HotKeyManager.SetHotKey(StationControl, new KeyGesture(Key.Tab));

        AddHandler(KeyDownEvent, StationWindow_KeyDown, RoutingStrategies.Tunnel);
    }

    private void StationStateWindow_Closed(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    protected void StationWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        StationControl.StationControl_KeyDown(this, e);
    }

    private void StationWindow_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        StationControl.Width = Width;
        StationControl.Height = Height;
    }

    private void StationWindow_Activated(object? sender, EventArgs e)
    {
        ((MainWindowViewModel)DataContext).SelectedStation = StationControl.Station;
    }

    public void MoveCars_Clicked()
    {
        MoveCarsDialogWindow moveCarsDialogWindow = new MoveCarsDialogWindow();

        moveCarsDialogWindow.ShowDialog(this);
    }

    public void UpdateTrack(Track selectedTrack)
    {
        if (selectedTrack.StationId == Station.StationId)
        {
            StationControl.UpdateTrack(selectedTrack.TrackNumber);
        }
        else
        {
            (DataContext as MainWindowViewModel).MainWindow.UpdateTrack(selectedTrack);
        }
    }
}