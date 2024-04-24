using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2;

public partial class IsCarLoadedDialogWindow : Window
{
    public IsCarLoadedDialogWindow(MainWindowViewModel dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
        var focusManager = this.FocusManager;
    }

    private void No_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
        viewModel.IsCarLoaded = false;
        CloseWindow();
    }

    private void Yes_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
        viewModel.IsCarLoaded = true;
        CloseWindow();
    }

    private void CloseWindow()
    {
        AddNewCarDialogWindow addCarDialogWindow = Owner as AddNewCarDialogWindow;
        addCarDialogWindow.addNewCar();
        Close();
    }
}