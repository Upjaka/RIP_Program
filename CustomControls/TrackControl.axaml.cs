using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using System.Diagnostics;

namespace AvaloniaApplication2.CustomControls;

public partial class TrackControl : UserControl
{
    public Track Track { get; set; }
    public static int CAR_WIDTH = 8;
    public static int TURNOUT_WIDTH = 40;

    public TrackControl()
    {
        InitializeComponent();
    }

    public TrackControl(Track track, MainWindowViewModel dataContext)
    {
        Track = track;
        InitializeComponent();
        DataContext = dataContext;
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
        }

        foreach (var car in track.Cars)
        {
            TrackGrid.Children.Add(new CarControl(car)
            {
                [Grid.RowProperty] = 0,
                [Grid.ColumnProperty] = car.SerialNumber - 1,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 2)
            });
        }
    }

    private void Track_MouseLeftButtonDown(object sender, PointerPressedEventArgs e)
    {
        if (sender is StackPanel stackPanel)
        {
            // Изменяем свойства рамки
            //stackPanel.BorderThickness = new Thickness(1);
            //stackPanel.BorderBrush = Brushes.Red;
        }
    }

    private void Track_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (((StackPanel)e.Source).Name == "Wrapper")
        {
            BorderWrapper.BorderThickness = new Thickness(1);
            if (DataContext != null)
            {
                var dataContext = (MainWindowViewModel)DataContext;
                dataContext.SelectedTrack = Track;
                Debug.WriteLine("Selected track: " + Track.ToString());
            }
        }      
    }

    private void Track_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (((StackPanel)e.Source).Name == "Wrapper")
        {
            BorderWrapper.BorderThickness = new Thickness(0);
            if (DataContext != null)
            {
                var dataContext = (MainWindowViewModel)DataContext;
                dataContext.SelectedTrack = null;
                Debug.WriteLine("Released track: " + Track.ToString());
            }
        }

    }

    private void Track_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Debug.WriteLine("Pointer Pressed! sender=" + sender.ToString());
    }
}