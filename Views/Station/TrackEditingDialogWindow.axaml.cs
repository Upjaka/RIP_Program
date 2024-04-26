using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using System;
using System.Diagnostics;

namespace AvaloniaApplication2;

public partial class TrackEditingDialogWindow : Window
{
    public TrackEditingDialogWindow() { InitializeComponent(); }

    public TrackEditingDialogWindow(MainWindowViewModel dataContext)
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
        Debug.WriteLine(dataContext.CarsInfo);
        Title = "Редактор района: " + dataContext.Station + " путь: " + dataContext.TrackNumber;
    }

    private void ExitButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}