using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using AvaloniaApplication2.Models;

namespace AvaloniaApplication2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greetings => "Welcome to Avalonia!";
        public string Loaded => "Груж.";
        public string NotLoaded => "Незагруж.";
        public string Station { get; set; }
        public string TrackNumber { get; set; }
        public bool IsCarLoaded { get; set; }
        public string CarNumber { get; set; }
        public int CarId { get; set; }
        public DateTime ComingDate { get; set; }
        public ObservableCollection<CarInfo> CarsInfo { get; }

        public MainWindowViewModel()
        {
            CarId = 1;
            Station = "";
            CarNumber = "";
            TrackNumber = "";
            var carsInfo = new List<CarInfo>
        {
            new CarInfo("1", "1111", true, "32", false, "1, 2", ""),
            new CarInfo("2", "1234", true, "10", false, "1,3", ""),
            new CarInfo("3", "4321", true, "31", false, "2, 3", "")
        };
            CarsInfo = new ObservableCollection<CarInfo>(carsInfo);
        }
    }
}
