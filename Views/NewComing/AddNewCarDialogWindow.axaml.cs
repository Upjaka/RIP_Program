using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Views;
using System.Collections.Generic;
using AvaloniaApplication2.Models;
using Avalonia.Input;
using System;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;
using iText.Layout.Element;
using Avalonia.Media;
using Org.BouncyCastle.Crmf;
using System.Linq;
using DynamicData;

namespace AvaloniaApplication2;

public partial class AddNewCarDialogWindow : Window
{
    private MainWindowViewModel viewModel;
    private List<Car> newCarsList = new List<Car>();
    private DateTime comingDateTime;
    public bool IsCarLoaded { get; set; }
    private string newCarNumber;
    private Dictionary<TextBox, Control[]> carGridRows;
    
    public AddNewCarDialogWindow()
    {
        carGridRows = new Dictionary<TextBox, Control[]>();

        Opened += (s, e) =>
        {
            viewModel = (MainWindowViewModel)Owner.DataContext;
            DataContext = viewModel;
            Title = $"Приход в район {viewModel.SelectedStation.StationName} на путь {viewModel.TrackNumber}";
        };

        InitializeComponent();

        AddEmptyRowToCarGrid();
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private async void ConfirmButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DateTimeDialogWindow dateTimeDialog = new DateTimeDialogWindow();

        dateTimeDialog.OkButton.Click += (s, e) =>
        {
            DateTime selectedDate = dateTimeDialog.DatePicker.SelectedDate.HasValue ? dateTimeDialog.DatePicker.SelectedDate.Value.Date : DateTime.MinValue;
            TimeSpan selectedTime = dateTimeDialog.TimePicker.SelectedTime ?? TimeSpan.Zero;
            comingDateTime = selectedDate + selectedTime;


            foreach (var textBox in carGridRows.Keys)
            {
                if (!String.IsNullOrEmpty(textBox.Text))
                {
                    int serialNumber = Convert.ToInt32((carGridRows[textBox][0] as TextBlock).Text);
                    bool isLoaded = (carGridRows[textBox][1] as CheckBox).IsChecked ?? false;
                    Car car = new Car(textBox.Text, serialNumber, false, "", isLoaded, "", "", comingDateTime, viewModel.NewComingTrack.TrackId);
                    newCarsList.Add(car);
                }
            } 
            viewModel.AddNewCar(newCarsList, InsertItBeginning.IsChecked ?? false);

            (Owner as MainWindow).UpdateTrack(viewModel.NewComingTrack);
            dateTimeDialog.Close();
            Close();
        };

        dateTimeDialog.CancelButton.Click += (s, e) =>
        {
            dateTimeDialog.Close();
        };

        var result = await dateTimeDialog.ShowDialog<bool>(this);
    }

    private TextBox AddEmptyRowToCarGrid()
    {
        TextBlock serialNumberTextBlock = new TextBlock()
        {
            [Grid.RowProperty] = CarGrid.RowDefinitions.Count,
            [Grid.ColumnProperty] = 1,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 0),
        };
        TextBox carNumberTextBox = new TextBox()
        {
            [Grid.RowProperty] = CarGrid.RowDefinitions.Count,
            [Grid.ColumnProperty] = 2,
            Watermark = "Введите номер вагона",
            BorderThickness = new Thickness(0),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Height = 30,
        };
        CheckBox isCarLoadedTextBlock = new CheckBox() 
        {
            [Grid.RowProperty] = CarGrid.RowDefinitions.Count,
            [Grid.ColumnProperty] = 3,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(0, 2, 0, 0),
            IsEnabled = false,
        };
        Button deleteButton = new Button()
        {
            [Grid.RowProperty] = CarGrid.RowDefinitions.Count,
            [Grid.ColumnProperty] = 0,
            Width = 20,
            Height = 20,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Background = Brushes.White,
            BorderThickness = new Thickness(0),
            Content = "x",
            Foreground = Brushes.Gray,
            IsVisible = false,
            Margin = new Thickness(0),
            Padding = new Thickness(2, 0, 2, 2)
        };

        carNumberTextBox.AddHandler(KeyDownEvent, CarGrigCell_KeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        deleteButton.Click += (s, e) =>
        {
            if (s is Button button)
            {
                int rowIndex = Grid.GetRow(button);

                foreach (var textBox in carGridRows.Keys)
                {
                    int row = Grid.GetRow(textBox);
                    if (row == rowIndex)
                    {
                        CarGrid.Children.Remove(textBox);
                        foreach (var control in carGridRows[textBox]) CarGrid.Children.Remove(control);
                    }
                    if (row > rowIndex)
                    {
                        Grid.SetRow(textBox, row - 1);
                        foreach (var control in carGridRows[textBox]) Grid.SetRow(control, row - 1);
                        (carGridRows[textBox][0] as TextBlock).Text = (row == CarGrid.RowDefinitions.Count - 1) ? "" : row.ToString();
                    }
                }

                CarGrid.RowDefinitions.RemoveAt(rowIndex);
            }
            
        };

        CarGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        CarGrid.Children.Add(serialNumberTextBlock);
        CarGrid.Children.Add(carNumberTextBox);
        CarGrid.Children.Add(isCarLoadedTextBlock);
        CarGrid.Children.Add(deleteButton);

        carGridRows[carNumberTextBox] = [serialNumberTextBlock, isCarLoadedTextBlock, deleteButton];

        return carNumberTextBox;
    }

    private async void CarGrigCell_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (e.Key == Key.Enter)
            {
                if (CheckUserInput(textBox))
                {
                    AddEmptyRowToCarGrid();

                }
                e.Handled = true;
            }
            if (e.Key == Key.V && e.KeyModifiers == KeyModifiers.Control)
            {
                var clipboard = GetTopLevel(this)?.Clipboard;
                var clipboardString = await clipboard.GetTextAsync();

                if (!string.IsNullOrEmpty(clipboardString))
                {
                    string[] splittedString = SplitClipboardText(clipboardString);

                    if (splittedString.Length == 1)
                    {
                        textBox.Text += splittedString[0];
                    }
                    else
                    {
                        int rowIndex = Grid.GetRow(textBox);
                        if (rowIndex == CarGrid.RowDefinitions.Count - 1) PasteData(textBox, splittedString);
                        else textBox.Text += splittedString[0];
                    }
                }

                e.Handled = true;
            }
        }
    }


    private bool CheckUserInput(TextBox textBox)
    {
        switch (viewModel.IsCarNumberCorrect(textBox.Text))
        {
            case 0:
                int rowIndex = Grid.GetRow(textBox) + 1;

                (carGridRows[textBox][0] as TextBlock).Text = rowIndex.ToString();
                (carGridRows[textBox][1] as CheckBox).IsEnabled = true;
                carGridRows[textBox][2].IsVisible = true;
                return true;

            case 1:
                IncorrectCarNumberDialogWindow carNumberDialogWindow = new IncorrectCarNumberDialogWindow(textBox.Text);
                carNumberDialogWindow.ShowDialog(this);
                return false;

            case 2:
                IncorrectCarNumberDialogWindow carNumberDialogWindow1 = new IncorrectCarNumberDialogWindow(textBox.Text, true);
                carNumberDialogWindow1.ShowDialog(this);
                return false;

            default:
                return false;
        }
    }

    private void PasteData(TextBox textBox, string[] data)
    {
        var currentTextBox = textBox;

        foreach (var str in data)
        {
            currentTextBox.Text += str;
            if (!CheckUserInput(currentTextBox)) break;
            currentTextBox = AddEmptyRowToCarGrid();
        }
    }

    private string[] SplitClipboardText(string clipboardString)
    {
        var result = new List<string>();
        string[] rows = clipboardString.Split('\n');
        foreach (var row in rows)
        {
            foreach (var str in row.Split('\r'))
            {
                if (!string.IsNullOrEmpty(str)) result.Add(str);
            }
        }
        return result.ToArray();
    }
}