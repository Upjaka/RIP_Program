using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication2;

public partial class LackOfSpaceOnTrackDialogWindow : Window
{
    public LackOfSpaceOnTrackDialogWindow()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}