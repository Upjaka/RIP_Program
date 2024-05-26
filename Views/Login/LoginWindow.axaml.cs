using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Views;

namespace AvaloniaApplication2;

public partial class LoginWindow : Window
{

    public LoginWindow()
    {
        InitializeComponent();

        Opened += (s, e) =>
        {
            UserNameComboBox.SelectionChanged += ComboBox_SelectionChanged;
            DataContext = (Owner.DataContext as MainWindowViewModel);
        };
    }

    private void LoginWindow_Opened(object? sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
            UserLevelTextBox.Watermark = (comboBox.SelectedIndex == 0) ? "Оператор" : "Наблюдатель";
    }

    private void LoginButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (DataContext as MainWindowViewModel).IsLoggedIn = true;
        (DataContext as MainWindowViewModel).IsOperator = (UserNameComboBox.SelectedIndex == 0) ? true : false;
        Close();
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}