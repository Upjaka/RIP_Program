using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2;

public partial class StationStateWindow : Window
{
    public StationStateWindow(MainWindowViewModel dataContext, string name)
    {
        InitializeComponent();
        Title = name;
    }
}