using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2;

public partial class IsCarLoadedDialogWindow : Window
{
    public IsCarLoadedDialogWindow() { InitializeComponent(); }

    public IsCarLoadedDialogWindow(MainWindowViewModel dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}