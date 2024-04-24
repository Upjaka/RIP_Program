using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2;

public partial class ChangeTrackDialogWindow : Window
{
    public ChangeTrackDialogWindow(MainWindowViewModel dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
        Title = "Change Track";
    }
}