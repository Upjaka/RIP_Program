using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Views;

namespace AvaloniaApplication2.CustomControls;

public partial class StationControl : UserControl
{
    MainWindow parent;

    public StationControl()
    {
        InitializeComponent();
    }

    public StationControl(MainWindow mainWindow, string title) 
    {
        InitializeComponent();
        parent = mainWindow;
        StationName.Text = title;
    }

    private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        parent.CloseStationControl(this);
    }
}