using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaApplication2.Models;
using System.Diagnostics;
using AvaloniaApplication2.Views;
using DynamicData;
using System.Data.SqlClient;
using Dapper;
using AvaloniaApplication2.CustomControls;

namespace AvaloniaApplication2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly string connectionToSQLServerString = "Server=alesantpc\\my_mssqlserver;Database=RIP;User Id=rip;Password=rip;";
        //private static readonly string connectionToSQLServerString = "Server=tob-rip-srv\\tob-rip-srv;Database=RIP;User Id=rip;Password=rip;";

        private ChangeList localChanges;

        public string Loaded => "Груж.";
        public string NotLoaded => "Незагруж.";
        public bool IsLoggedIn {  get; set; }
        public bool IsOperator { get; set; }
        public MainWindow MainWindow { get; set; }        
        public string TrackNumber { get; set; }
        public Track NewComingTrack { get; set; }
        public ObservableCollection<CarInfo> CarsInfo { get; }
        public List<CarInfo> OldCarsInfo { get; }
        public ObservableCollection<DefectCode> DefectCodes { get; }
        public List<Station> Stations { get; } = new List<Station>();
        private Station? _selectedStation = null;
        public Station ? SelectedStation
        {
            get => _selectedStation;
            set
            {
                if (_selectedStation != value)
                {
                    _selectedStation = value;
                    MainWindow.SetMenuItemsEnabling();

                }
            }
        }
        private Track? _selectedTrack = null;
        public Track? SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                if (_selectedTrack != value)
                {
                    _selectedTrack = value;
                    MainWindow.SetMenuItemsEnabling();
                }
            }
        }
        public List<Car> movingCarsList {  get; set; }
        public Car LastFocusedCar { get; set; }
        public Dictionary<Station, StationControl> OpenedStations;

        public string ServerAddress {  get; set; }
        public string ServerUser { get; set; }
        public string ServerPassword { get; set; }


        public bool HasUnsavedChanges
        {
            get { return localChanges.Count > 0; }
        }

        public MainWindowViewModel()
        {
            TrackNumber = "";       

            CarsInfo = new ObservableCollection<CarInfo>();

            OldCarsInfo = new List<CarInfo>();

            DefectCodes = new ObservableCollection<DefectCode>();

            OpenedStations = new Dictionary<Station, StationControl>();

            localChanges = new ChangeList();
        }

        private string GetConnectionString()
        {
            return $"Server={ServerAddress};Database=RIP;User Id={ServerUser};Password={ServerPassword};";
        }

        private void OnCarChanged(object sender, CarChangedEventArgs e)
        {
            localChanges.Add(new LocalChange(e.OldCar, e.NewCar));
        }

        public void UpdateCarsInfo() 
        {
            CarsInfo.Clear();
            OldCarsInfo.Clear();

            if (SelectedTrack != null)
            {
                foreach (var c in SelectedTrack.Cars)
                {
                    CarsInfo.Add(new CarInfo(c));
                    OldCarsInfo.Add(new CarInfo(c));
                }
            }
        }

        public void CancelChanges()
        {
            CarsInfo.Clear();
            foreach (var c in OldCarsInfo)
            {
                CarsInfo.Add(c);
            }
        }

        public bool HasChanges()
        {
            for (int i = 0; i < CarsInfo.Count; i++)
            {
                CarInfo carInfo = CarsInfo[i];
                if (!carInfo.Equals(OldCarsInfo[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public int IsCarNumberCorrect(string number)
        {
            if (number.Length != 8) return 1;

            bool isNumber = int.TryParse(number, out _);

            if (isNumber)
            {
                var controlCode = 0;

                for (int i = 0; i < 7; i++)
                {
                    var code = Convert.ToInt32(number[i].ToString()) * (2 - i % 2);

                    controlCode += (code < 10) ? code : code % 10 + code / 10;
                }

                if ((10 - controlCode % 10) % 10 != Convert.ToInt32(number) % 10) return 1;

                foreach (var station in Stations)
                {
                    foreach (var track in station.Tracks)
                    {
                        foreach (var car in track.Cars)
                        {
                            if (car.CarNumber == number)
                            {
                                return 2;
                            }
                        }
                    }
                }
                return 0;
            }
            else
            {
                return 1;
            }            
        }

        public void AddNewCar(List<Car> cars, bool inBeginning)
        {
            int startIndex = NewComingTrack.Cars.Count;

            foreach (var car in cars)
            {
                if (!inBeginning) car.SerialNumber += startIndex;
                car.CarChanged += OnCarChanged;
                if (!inBeginning) NewComingTrack.AddLast(car);
                else NewComingTrack.AddFirst(new List<Car>([car]));
                LocalChange change = new LocalChange(car);
                localChanges.Add(change);
            }            
        }

        public void ConfirmChanges()
        {
            for (int i = 0; i < CarsInfo.Count; i++)
            {
                CarInfo carInfo = CarsInfo[i];
                if (!carInfo.Equals(OldCarsInfo[i]))
                {
                    Car? car = SelectedTrack.GetCar(Convert.ToInt32(CarsInfo[i].SerialNumber));
                    if (car != null)
                    {
                        Car oldCar = car.Clone();
                        car.IsFixed = carInfo.IsFixed;
                        car.IsLoaded = carInfo.IsLoaded;
                        car.DefectCodes = carInfo.DefectCodes;
                        car.Product = carInfo.Product;
                        car.Cargo = carInfo.Cargo;
                        car.IsSelected = carInfo.IsSelected;
                        localChanges.Add(new LocalChange(oldCar, car.Clone()));
                    }
                }
            }
            CarsInfo.Clear();
            OldCarsInfo.Clear();
        }

        public void ClearChanges()
        {
            localChanges.Clear();
        }


        public Station GetStationByName(string stationName)
        {
            foreach (Station station in Stations)
            {
                if (station.StationName == stationName) return station;
            }
            return null;
        }

        public int GetTrackIdByNumber(int n)
        {
            foreach (Station station in Stations)
            {
                foreach (Track track in station.Tracks)
                {
                    if (track.TrackNumber == n) return track.TrackId;
                }
            }
            return 0;
        }

        public bool MoveCars(Track destinationTrack, bool IsFirst = false)
        {
            List<Car> selectedCars = SelectedTrack.GetSelectedCars();

            if (destinationTrack.Cars.Count + selectedCars.Count <= destinationTrack.Capacity)
            {
                List<Car> oldSelectedCars = new List<Car>();
                foreach (Car car in selectedCars) oldSelectedCars.Add(car.Clone());

                SelectedTrack.RemoveAllCars(selectedCars);

                if (!IsFirst)
                {
                    destinationTrack.AddLast(selectedCars);
                }
                else
                {
                    destinationTrack.AddFirst(selectedCars);
                }

                for (int i = 0; i < selectedCars.Count; i++)
                {
                    localChanges.Add(new LocalChange(oldSelectedCars[i], selectedCars[i]));
                }
                return true;
            }
            else return false;
        }

        public void ConnectToDefectCodesDatabase()
        {
            using (var connection = new SqlConnection(connectionToSQLServerString))
            {
                try
                {
                    var queryString = "SELECT * FROM DefectCodes";
                    connection.Open();
                    var defectCodes = connection.Query<DefectCode>(queryString).AsList();

                    DefectCodes.AddRange(defectCodes);

                    MainWindow.StatusBarTextBlock.Text = "Подключено";
                }
                catch (SqlException ex)
                {
                    MainWindow.StatusBarTextBlock.Text =
                        "Ошибка при получении кодов брака:" + ex.Message;
                    Debug.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
        }

        public void ConnectToStationsDatabase()
        {
            using (var connection = new SqlConnection(connectionToSQLServerString))
            {
                try
                {
                    connection.Open();

                    // Получаем все станции
                    var getAllStationsQueryString = "SELECT * FROM Stations";
                    var stations = connection.Query<Station>(getAllStationsQueryString).AsList();
                    Stations.AddRange(stations);

                    foreach (var station in Stations)
                    {
                        // Получаем все треки для текущей станции
                        var getTracksFromStationQueryString = "SELECT * FROM Tracks WHERE StationID = @StationId";
                        var tracks = connection.Query<Track>(getTracksFromStationQueryString, new { StationId = station.StationId }).AsList();
                        station.AddTracks(tracks);

                        foreach (var track in station.Tracks)
                        {
                            // Получаем все вагоны для текущего трека
                            var getCarsFromTrackQueryString = "SELECT * FROM Cars WHERE TrackID = @TrackId";
                            var cars = connection.Query<Car>(getCarsFromTrackQueryString, new { TrackId = track.TrackId }).AsList();

                            foreach (var car in cars)
                            {
                                car.CarChanged += OnCarChanged;
                                track.AddLast(car);
                            }

                            track.Sort();
                        }
                    }

                    MainWindow.StatusBarTextBlock.Text = "Подключено";
                    Console.WriteLine("Data loaded successfully.");
                }
                catch (SqlException ex)
                {
                    MainWindow.StatusBarTextBlock.Text = "Ошибка при получении данных по вагонам:" + ex.Message;
                    Debug.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
        }

        public bool SaveChangesToDataBase()
        {
            if (localChanges.Count > 0)
            {
                using (var connection = new SqlConnection(connectionToSQLServerString))
                {
                    try
                    {
                        connection.Open();

                        foreach (var updateRequestString in localChanges.GetSQLRequests())
                        {
                            try
                            {
                                int rowsAffected = connection.Execute(updateRequestString);

                                if (rowsAffected > 0)
                                {
                                    Debug.WriteLine("Запись успешно добавлена в базу данных.");
                                }
                                else
                                {
                                    Debug.WriteLine("Ошибка при добавлении записи в базу данных.");
                                }
                            }
                            catch (SqlException ex)
                            {
                                Debug.WriteLine("Ошибка при выполнении запроса: " + ex.Message);
                            }
                        }

                        localChanges.Clear();

                        MainWindow.StatusBarTextBlock.Text = "";
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        MainWindow.StatusBarTextBlock.Text = "Ошибка сохранения данных:" + ex.Message;
                        Debug.WriteLine("Error connecting to the database: " + ex.Message);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }        
    }
}
