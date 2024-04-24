using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using System;

namespace AvaloniaApplication2;

public partial class DateTimeDialogWindow : Window
{
    public DateTimeDialogWindow(MainWindowViewModel dataContext)
    {
        InitializeComponent();
        DataContext = dataContext; 
        DatePicker.SelectedDate = DateTime.Now;
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
        DateTime selectedDate = DatePicker.SelectedDate.HasValue ? DatePicker.SelectedDate.Value.Date : DateTime.MinValue;
        TimeSpan selectedTime = TimePicker.SelectedTime ?? TimeSpan.Zero;
        viewModel.ComingDate = selectedDate + selectedTime;
        AddNewCarDialogWindow addCarDialogWindow = Owner as AddNewCarDialogWindow;
        addCarDialogWindow.Close();
        Close();
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}