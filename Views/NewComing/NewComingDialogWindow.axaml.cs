using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Views;
using System;
using System.Diagnostics;

namespace AvaloniaApplication2;

public partial class NewComingDialogWindow : Window
{
    private MainWindow mainWindow;
    private const int DOUBLETAP_INTERVAL = 300;
    private Stopwatch _stopwatch = new Stopwatch();

    public NewComingDialogWindow(MainWindow mainWindow)
    {
        InitializeComponent();
        this.mainWindow = mainWindow;
    }

    private void Track_DoubleTapped(object sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            if (sender is TextBlock textBlock)
            {
                string buttonContent = textBlock.Text.ToString();

                if (mainWindow != null)
                {
                    mainWindow.UpdateButtonContent(buttonContent);
                    MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
                    if (viewModel != null)
                    {
                        viewModel.TrackNumber = buttonContent;
                    }
                }
            }
            CloseWindow();
        }
    }

    private void SaveTrackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CloseWindow();
    }

    private void CloseWindow()
    {
        var newComing_AddCarDialogWindow = new AddNewCarDialogWindow(mainWindow, (MainWindowViewModel)this.DataContext);
        newComing_AddCarDialogWindow.ShowDialog(mainWindow);
        Close();
    }

    /**
    // Метод для вызова события при закрытии окна с передачей данных
    private void OnDialogClosed(TrackComing data)
    {
        DialogClosed?.Invoke(this, data);
    }
    */
}