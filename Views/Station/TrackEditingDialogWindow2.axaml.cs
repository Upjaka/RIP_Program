using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using System;
using System.Diagnostics;

namespace AvaloniaApplication2;

public partial class TrackEditingDialogWindow2 : Window
{
    public TrackEditingDialogWindow2()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
    }

    public TrackEditingDialogWindow2(MainWindowViewModel dataContext)
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

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        Console.WriteLine("Window initialized");
        Console.WriteLine("DataContext: " + DataContext); // Проверка DataContext
        Debug.WriteLine("Window initialized");
        Debug.WriteLine("DataContext: " + ((MainWindowViewModel)DataContext).CarsInfo[0].ToString()); // Проверка DataContext
    }
}