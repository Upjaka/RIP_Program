using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2;

public partial class MoveCarsDialogWindow : Window
{
    private MainWindowViewModel viewModel;

    public MoveCarsDialogWindow()
    {
        InitializeComponent();

        Opened += (s, e) =>
        {
            viewModel = (MainWindowViewModel)Owner.DataContext;
            DataContext = viewModel;

            Title = $"Перем. из: { viewModel.SelectedStation.StationName } путь { viewModel.SelectedTrack.TrackNumber }";
        };
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}