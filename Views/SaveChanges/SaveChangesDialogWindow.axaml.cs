using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

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

    private void No_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewModel.CancelChanges();
        CloseWindow();
    }

    private void Yes_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewModel.ConfirmChanges();
        CloseWindow();
    }

    private void CloseWindow()
    {
        Close();
    }
}