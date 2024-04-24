using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2;

public partial class ChangeTrackDialogWindow : Window
{
    public ChangeTrackDialogWindow() { InitializeComponent(); }
    public ChangeTrackDialogWindow(MainWindowViewModel dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
        Title = "Change Track";
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