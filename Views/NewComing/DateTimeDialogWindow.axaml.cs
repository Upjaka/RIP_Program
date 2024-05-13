using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using System;

namespace AvaloniaApplication2;

public partial class DateTimeDialogWindow : Window
{
    public DateTimeDialogWindow()
    {
        InitializeComponent();
        DatePicker.SelectedDate = DateTime.Now;
    }
}