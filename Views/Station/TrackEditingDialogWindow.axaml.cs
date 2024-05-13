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

    private async void ExitButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
        
        if (viewModel.HasChanges())
        {
            SaveChangesDialogWindow saveChangesDialogWindow = new SaveChangesDialogWindow((MainWindowViewModel)DataContext);
            saveChangesDialogWindow.YesButton.Click += (s, e) =>
            {
                ((MainWindowViewModel)DataContext).ConfirmChanges();
                saveChangesDialogWindow.Close();
                mainWindow.UpdateSelectedTrack(viewModel.SelectedTrack.TrackId);
                Close();
            };
            saveChangesDialogWindow.NoButton.Click += (s, e) =>
            {
                ((MainWindowViewModel)DataContext).CancelChanges();
                saveChangesDialogWindow.Close();
                mainWindow.UpdateSelectedTrack(viewModel.SelectedTrack.TrackId);
                Close();
            };
            saveChangesDialogWindow.CancelButton.Click += (s, e) =>
            {
                saveChangesDialogWindow.Close();
            };
            await saveChangesDialogWindow.ShowDialog<bool>(this);
        }
    }

    private async void TrackEditing_Closing(object? sender, Avalonia.Controls.WindowClosingEventArgs args)
    {
        MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

        if (viewModel.HasChanges())
        {
            args.Cancel = true;

            SaveChangesDialogWindow saveChangesDialogWindow = new SaveChangesDialogWindow(viewModel);
            saveChangesDialogWindow.YesButton.Click += (s, e) =>
            {
                viewModel.ConfirmChanges();
                saveChangesDialogWindow.Close();
                mainWindow.UpdateSelectedTrack(viewModel.SelectedTrack.TrackId);
                Close();
            };
            saveChangesDialogWindow.NoButton.Click += (s, e) =>
            {
                viewModel.CancelChanges();
                saveChangesDialogWindow.Close();
                Close();
            };
            saveChangesDialogWindow.CancelButton.Click += (s, e) =>
            {
                saveChangesDialogWindow.Close();
            };
            await saveChangesDialogWindow.ShowDialog<bool>(this);
        }
    }
}