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
        InfoPanelTextBlock.Text = (isNumberExists) ? $"����� � ������� '{carNumber}' ��� ����������" : $"����� ������ '{ carNumber }' �� �������� ��������";
    }

    private void YesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}