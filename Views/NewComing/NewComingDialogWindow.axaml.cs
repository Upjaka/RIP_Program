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

    public NewComingDialogWindow(MainWindow mainWindow, int selectedTrack = 0)
    {
        InitializeComponent();
        this.mainWindow = mainWindow;
        DataContext = mainWindow.DataContext;
        AddTracksToTracksPanel(selectedTrack);

        KeyDown += NewComing_KeyDown;
    }

    private void SaveTrackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SelectTrack();
        Close();
    }
    
    private void AddTracksToTracksPanel(int selectedTrack)
    {
        MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

        foreach (var track in viewModel.SelectedStation.Tracks)
        {
            Border trackBorder = new Border() 
            { 
                Name = "TrackBorder" + track.TrackNumber,
                Classes = { "TrackNumberBorder" }
            };
            TextBlock trackNumberTextBlock = new TextBlock()
            {
                Text = track.TrackNumber.ToString(),
                Classes = { "TrackNumberTextBlock" },
                Margin = new Thickness(3, 0, 0, 0)
            };

            trackBorder.PointerPressed += TrackBorder_PointerPressed;

            trackBorder.Child = trackNumberTextBlock;            
            TrackNumbersPanel.Children.Add(trackBorder);

            if (track.TrackNumber == selectedTrack)
            {
                trackBorder.Classes.Add("Selected");
            }
        }
    }

    private void TrackBorder_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        TextBlock textBlock = ((Border)sender).Child as TextBlock;
        ((MainWindowViewModel)DataContext).TrackNumber = textBlock.Text;
        
        foreach (Border trackBorder in TrackNumbersPanel.Children)
        {
            Debug.WriteLine(trackBorder.Classes);
            trackBorder.Classes.Remove("Selected");
        }
        ((Border)sender).Classes.Add("Selected");

        if (e.ClickCount == 2)
        {
            SelectTrack();
            Close();
        }
    }

    private void NewComing_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                SelectTrack();
                Close();
                break;
            case Key.Tab:
                if (e.KeyModifiers == KeyModifiers.Shift)
                {
                    SelectPreviousTrack();
                }
                else
                {
                    SelectNextTrack();
                }
                break;

            case Key.Down:
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    SelectNextTrack();
                }
                break;

            case Key.Up:
                if (e.KeyModifiers == KeyModifiers.None)
                {
                    SelectPreviousTrack();
                }
                break;
        }
    }

    public void SelectNextTrack()
    {
        Controls tracks = TrackNumbersPanel.Children;
        bool selected = false;

        for (int i = 0; i < TrackNumbersPanel.Children.Count; i++)
        {
            if (((Border)tracks[i]).Classes.Contains("Selected"))
            {
                ((Border)tracks[i]).Classes.Remove("Selected");
                ((Border)tracks[(i + 1) % tracks.Count]).Classes.Add("Selected");
                ((Border)tracks[(i + 1) % tracks.Count]).BringIntoView();
                selected = true;
                break;
            }
        }
        if (!selected)
        {
            ((Border)tracks[0]).Classes.Add("Selected");
        }
    }

    public void SelectPreviousTrack()
    {
        Controls tracks = TrackNumbersPanel.Children;
        bool selected = false;

        for (int i = tracks.Count - 1; i >= 0; i--)
        {
            if (((Border)tracks[i]).Classes.Contains("Selected"))
            {
                ((Border)tracks[i]).Classes.Remove("Selected");
                ((Border)tracks[(i - 1 + tracks.Count) % tracks.Count]).Classes.Add("Selected");
                ((Border)tracks[(i - 1 + tracks.Count) % tracks.Count]).BringIntoView();
                selected = true;
                break;
            }
        }
        if (!selected)
        {
            ((Border)tracks[tracks.Count - 1]).Classes.Add("Selected");
        }
    }

    private void SelectTrack()
    {
        MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

        foreach (Border trackBorder in TrackNumbersPanel.Children)
        {
            if (trackBorder.Classes.Contains("Selected"))
            {
                viewModel.NewComingTrack = viewModel.SelectedStation.GetTrackByNumber(Convert.ToInt32(((TextBlock)trackBorder.Child).Text));
            }
        }

        AddNewCarDialogWindow addNewCarWindow = new AddNewCarDialogWindow();
        addNewCarWindow.ShowDialog(mainWindow);
    }
}