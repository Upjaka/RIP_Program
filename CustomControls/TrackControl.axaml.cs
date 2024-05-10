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
    public Track Track { get; }
    private StationControl parent;
    public static int CAR_WIDTH = 8;
    public static int TURNOUT_WIDTH = 40;

    public TrackControl()
    {
        InitializeComponent();
    }

    public TrackControl(Track track, StationControl parentStation)
    {
        Track = track;
        parent = parentStation;


        InitializeComponent();

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

    private void Track_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        MakeSelected();
        if (DataContext != null)
        {
            parent.SelectTrack(Track);
            Debug.WriteLine("Selected track: " + Track.ToString());
        }
    }

    public void MakeSelected()
    {
        BorderWrapper.BorderThickness = new Thickness(1);
    }

    public void MakeUnselected()
    {
        BorderWrapper.BorderThickness = new Thickness(0);
    }


    public void UpdateTrack()
    {
        foreach (CarControl carControl in TrackGrid.Children)
        {
            carControl.UpdateCar();
        }
    }
}