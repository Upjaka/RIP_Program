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

namespace AvaloniaApplication2;

public partial class AddNewCarDialogWindow : Window
{
    private MainWindowViewModel viewModel;
    private List<Car> newCarsList = new List<Car>();
    private DateTime comingDateTime;
    public bool IsCarLoaded { get; set; }
    private string newCarNumber;
    
    public AddNewCarDialogWindow()
    {
        Opened += (s, e) =>
        {
            viewModel = (MainWindowViewModel)Owner.DataContext;
            DataContext = viewModel;
            Title = $"Приход в район {viewModel.SelectedStation.StationName} на путь {viewModel.TrackNumber}";
        };

        InitializeComponent();
    }

    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.None)
        {
            viewModel.CarNumber = CarNumberTextBox.Text;
            if (viewModel.IsCarNumberCorrect(Convert.ToInt32(CarNumberTextBox.Text)))
            {
                newCarNumber = CarNumberTextBox.Text;

                CarNumberTextBox.Clear();
                IsCarLoadedDialogWindow isCarLoadedDialogWindow = new IsCarLoadedDialogWindow(viewModel);
                isCarLoadedDialogWindow.YesButton.Click += (s, e) =>
                {
                    IsCarLoaded = true;
                    AddNewCar();
                    isCarLoadedDialogWindow.Close();
                };
                isCarLoadedDialogWindow.NoButton.Click += (s, e) =>
                {
                    IsCarLoaded = false;
                    AddNewCar();
                    isCarLoadedDialogWindow.Close();
                };
                isCarLoadedDialogWindow.ShowDialog(this);
            }
            else
            {
                IncorrectCarNumberDialogWindow carNumberDialogWindow = new IncorrectCarNumberDialogWindow(CarNumberTextBox.Text);
                carNumberDialogWindow.ShowDialog(this);
            }
        }
    }

    public void AddNewCar()
    {
        int newCarSerialNumber = (viewModel.NewComingTrack.Cars.Count == 0) ? 1 : viewModel.NewComingTrack.Cars.Last.Value.SerialNumber + 1;
        CarGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        TextBlock carNumberTextBlock = new TextBlock();
        carNumberTextBlock.Text = newCarNumber;
        TextBlock carSerialNumberTextBlock = new TextBlock();
        carSerialNumberTextBlock.Text = newCarNumber.ToString();
        TextBlock isCarLoadedTextBlock = new TextBlock();
        isCarLoadedTextBlock.Text = (IsCarLoaded) ? viewModel.Loaded : viewModel.NotLoaded;
        
        Grid.SetRow(carNumberTextBlock, CarGrid.RowDefinitions.Count - 1);
        Grid.SetRow(carSerialNumberTextBlock, CarGrid.RowDefinitions.Count - 1);
        Grid.SetRow(isCarLoadedTextBlock, CarGrid.RowDefinitions.Count - 1);
        Grid.SetColumn(carNumberTextBlock, 0); 
        Grid.SetColumn(carSerialNumberTextBlock, 1);
        Grid.SetColumn(isCarLoadedTextBlock, 2);
        CarGrid.Children.Add(carNumberTextBlock);
        CarGrid.Children.Add(carSerialNumberTextBlock);
        CarGrid.Children.Add(isCarLoadedTextBlock);

        Car newCar = new Car(newCarNumber, newCarSerialNumber, false, "", IsCarLoaded, "", "", DateTime.Now, viewModel.NewComingTrack.TrackId);
        newCarsList.Add(newCar);
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

            foreach (Car car in newCarsList)
            {
                car.Arrival = comingDateTime;
            }
            viewModel.AddNewCar(newCarsList);

            ((MainWindow)Owner).UpdateSelectedTrack(viewModel.NewComingTrack.TrackNumber);
            dateTimeDialog.Close();
            Close();
        };

        dateTimeDialog.CancelButton.Click += (s, e) =>
        {
            dateTimeDialog.Close();
        };

        var result = await dateTimeDialog.ShowDialog<bool>(this);
    }
}