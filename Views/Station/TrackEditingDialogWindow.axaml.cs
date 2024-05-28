using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Models;
using AvaloniaApplication2.ViewModels;
using System;
using System.Diagnostics;
using AvaloniaApplication2.Views;
using Avalonia.Input;
using Avalonia.Interactivity;

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

        if (dataContext.SelectedTrack != null)
        {
            Title = "Редактор района: " + dataContext.SelectedStation.StationName + " путь: " + dataContext.SelectedTrack.TrackNumber;
        }

        TrackGrid.CurrentCellChanged += (sender, e) =>
        {
            DataGridColumn gridColumn = TrackGrid.CurrentColumn;

            if (gridColumn != null)
            {
                if (!gridColumn.IsReadOnly)
                {
                    if (gridColumn is DataGridTextColumn) 
                        TrackGrid.BeginEdit();
                }
            }            
        };
        
        TrackGrid.AddHandler(KeyDownEvent, TrackGrid_KeyDown, RoutingStrategies.Tunnel);
    }

    private async void ExitButton_Click(object? sender, RoutedEventArgs e)
    {
        MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
        
        if (viewModel.HasChanges())
        {
            SaveChangesDialogWindow saveChangesDialogWindow = new SaveChangesDialogWindow((MainWindowViewModel)DataContext);
            saveChangesDialogWindow.YesButton.Click += (s, e) =>
            {
                ((MainWindowViewModel)DataContext).ConfirmChanges();
                saveChangesDialogWindow.Close();
                mainWindow.UpdateTrack(viewModel.SelectedTrack);
                Close();
            };
            saveChangesDialogWindow.NoButton.Click += (s, e) =>
            {
                ((MainWindowViewModel)DataContext).CancelChanges();
                saveChangesDialogWindow.Close();
                mainWindow.UpdateTrack(viewModel.SelectedTrack);
                Close();
            };
            saveChangesDialogWindow.CancelButton.Click += (s, e) =>
            {
                saveChangesDialogWindow.Close();
            };
            await saveChangesDialogWindow.ShowDialog<bool>(this);
        }
        else
        {
            Close();
        }
    }

    private async void TrackEditing_Closing(object? sender, WindowClosingEventArgs args)
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
                mainWindow.UpdateTrack(viewModel.SelectedTrack);
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

    private void TrackGrid_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Down)
        {
            TrackGrid.CommitEdit();

            var trackGridColumn = TrackGrid.CurrentColumn;

            TrackGrid.SelectedIndex += 1;

            if (TrackGrid.SelectedIndex == -1)
            {
                TrackGrid.SelectedIndex = 0;

                TrackGrid.CurrentColumn = trackGridColumn;
            }

            TrackGrid.ScrollIntoView(TrackGrid.SelectedItem, trackGridColumn);

            TrackGrid.BeginEdit();

            e.Handled = true;
        }
        if (e.Key == Key.Up)
        {
            TrackGrid.CommitEdit();

            var trackGridColumn = TrackGrid.CurrentColumn;

            TrackGrid.SelectedIndex -= 1;

            if (TrackGrid.SelectedIndex == -1)
            {
                TrackGrid.SelectedIndex = (DataContext as MainWindowViewModel).CarsInfo.Count - 1;

                TrackGrid.CurrentColumn = trackGridColumn;
            }

            TrackGrid.ScrollIntoView(TrackGrid.SelectedItem, trackGridColumn);

            TrackGrid.BeginEdit();

            e.Handled = true;
        }
    }

    private void Window_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
    }
}