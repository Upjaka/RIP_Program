using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2;

public partial class DefectCodesDialogWindow : Window
{
    public DefectCodesDialogWindow()
    {
        InitializeComponent();
    }

    public DefectCodesDialogWindow(MainWindowViewModel dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}