using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Views;

namespace AvaloniaApplication2;

public partial class AddNewCarDialogWindow : Window
{
    private MainWindowViewModel viewModel;
    
    public AddNewCarDialogWindow(MainWindow mainWindow, MainWindowViewModel dataContext)
    {
        InitializeComponent();
        Title = "Приход в район ... на путь " + dataContext.TrackNumber;
        this.DataContext = dataContext;
        viewModel = (MainWindowViewModel)DataContext;
    }

    private void TextBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter)
        {
            viewModel.CarNumber = CarNumberTextBox.Text;
            CarNumberTextBox.Clear();
            //IsCarLoadedDialogWindow isCarLoadedDialogWindow = new IsCarLoadedDialogWindow((MainViewModel)this.DataContext);
            //isCarLoadedDialogWindow.ShowDialog(this);
        }
    }

    public void addNewCar()
    {
        CarGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        TextBlock carNumberTextBlock = new TextBlock();
        carNumberTextBlock.Text = viewModel.CarNumber;
        TextBlock carIdTextBlock = new TextBlock();
        carIdTextBlock.Text = viewModel.CarId.ToString();
        TextBlock isCarLoadedTextBlock = new TextBlock();
        if (viewModel.IsCarLoaded)
        {
            isCarLoadedTextBlock.Text = viewModel.Loaded;
        }
        else
        {
            isCarLoadedTextBlock.Text = viewModel.NotLoaded;
        }
        Grid.SetRow(carNumberTextBlock, viewModel.CarId - 1);
        Grid.SetRow(carIdTextBlock, viewModel.CarId - 1);
        Grid.SetRow(isCarLoadedTextBlock, viewModel.CarId - 1);
        Grid.SetColumn(carNumberTextBlock, 0);
        Grid.SetColumn(carIdTextBlock, 1);
        Grid.SetColumn(isCarLoadedTextBlock, 2);
        CarGrid.Children.Add(carNumberTextBlock);
        CarGrid.Children.Add(carIdTextBlock);
        CarGrid.Children.Add(isCarLoadedTextBlock);
        viewModel.CarId += 1;
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private void ConfirmButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DateTimeDialogWindow dateTimeDialog = new DateTimeDialogWindow((MainWindowViewModel)DataContext);
        dateTimeDialog.ShowDialog(this);
    }
}