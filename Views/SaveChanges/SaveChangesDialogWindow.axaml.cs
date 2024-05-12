using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace AvaloniaApplication2;

public partial class SaveChangesDialogWindow : Window
{
    MainWindowViewModel viewModel;

    public SaveChangesDialogWindow()
    {
        InitializeComponent();
    }

    public SaveChangesDialogWindow(MainWindowViewModel dataContext)
    {
        viewModel = dataContext;
        InitializeComponent();
    }
}