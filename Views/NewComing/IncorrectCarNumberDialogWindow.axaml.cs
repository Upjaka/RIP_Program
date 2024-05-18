using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication2;

public partial class IncorrectCarNumberDialogWindow : Window
{
    public IncorrectCarNumberDialogWindow()
    {
        InitializeComponent();
    }

    public IncorrectCarNumberDialogWindow(string carNumber, bool isNumberExists = false)
    {
        InitializeComponent();
        InfoPanelTextBlock.Text = (isNumberExists) ? $"Вагон с нмоером '{carNumber}' уже существует" : $"Номер вагона '{ carNumber }' не проходит контроль";
    }

    private void YesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}