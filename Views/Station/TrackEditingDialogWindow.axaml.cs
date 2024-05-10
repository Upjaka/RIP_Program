using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using System;
using System.Diagnostics;
using AvaloniaApplication2.Views;

namespace AvaloniaApplication2;

public partial class TrackEditingDialogWindow : Window
{
    MainWindow mainWindow;

    public TrackEditingDialogWindow() { InitializeComponent(); }

    public TrackEditingDialogWindow(MainWindow mainWindow, MainWindowViewModel dataContext)
    {
        this.mainWindow = mainWindow;
        DataContext = dataContext;
        InitializeComponent();
        Debug.WriteLine(dataContext.CarsInfo);
        if (dataContext.SelectedTrack != null)
        {
            Title = "Редактор района: " + dataContext.SelectedStation.StationName + " путь: " + dataContext.SelectedTrack.TrackNumber;
        }
    }

    private void ExitButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SaveChangesDialogWindow saveChangesDialogWindow = new SaveChangesDialogWindow((MainWindowViewModel)DataContext);
        saveChangesDialogWindow.Closed += (sender, args) =>
        {
            mainWindow.UpdateSelectedTrack();
            Close();
        };
        saveChangesDialogWindow.ShowDialog(this);
    }
}