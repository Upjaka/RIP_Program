using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using AvaloniaApplication2.Models;
using System.Diagnostics;
using AvaloniaApplication2.Views;
using DynamicData;

namespace AvaloniaApplication2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly string connectionToDefectCodesDbString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\User\source\repos\AvaloniaApplication2\DefectCodes.accdb;";
        private static readonly string connectionToStationsDbString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\User\source\repos\AvaloniaApplication2\Stations.accdb;";

        private ChangeList localChanges;

        public string Greetings => "Welcome to Avalonia!";
        public string Loaded => "Груж.";
        public string NotLoaded => "Незагруж.";
        public bool IsLoggedIn {  get; set; }
        public MainWindow MainWindow { get; set; }
        public string Station { get; set; }
        public string TrackNumber { get; set; }
        public Track NewComingTrack { get; set; }
        public bool IsCarLoaded { get; set; }
        public string CarNumber { get; set; }
        public int CarId { get; set; }
        public DateTime ComingDate { get; set; }
        public ObservableCollection<CarInfo> CarsInfo { get; }
        public List<CarInfo> OldCarsInfo { get; }
        public ObservableCollection<DefectCode> DefectCodes { get; }
        public List<Station> Stations { get; } = new List<Station>();
        public Track ?SelectedTrack { get; set; } = null;
        public Station ?SelectedStation { get; set; } = null;
        public List<Car> movingCarsList {  get; set; }
        public Car LastFocusedCar { get; set; }


        public bool HasUnsavedChanges
        {
            get { return localChanges.Count > 0; }
        }

        public MainWindowViewModel()
        {
            CarId = 1;
            Station = "";
            CarNumber = "";
            TrackNumber = "";       

            CarsInfo = new ObservableCollection<CarInfo>();

            OldCarsInfo = new List<CarInfo>();

            DefectCodes = new ObservableCollection<DefectCode>();

            localChanges = new ChangeList();

            ConnectToDefectCodesDatabase();

            ConnectToStationsDatabase();
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

                if (10 - controlCode % 10 != Convert.ToInt32(number) % 10) return 1;

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

        public bool SaveChangesToDataBase()
        {
            if (localChanges.Count > 0)
            {
                using (OleDbConnection connection = new OleDbConnection(connectionToStationsDbString))
                {
                    try
                    {
                        connection.Open();

                        foreach (var updateRequestString in localChanges.GetSQLRequests())
                        {
                            using (OleDbCommand command = new OleDbCommand(updateRequestString, connection))
                            {
                                // Выполнение запроса
                                int rowsAffected = command.ExecuteNonQuery();

                                // Проверка на количество затронутых строк (обычно 1)
                                if (rowsAffected > 0)
                                {
                                    Debug.WriteLine("Запись успешно добавлена в базу данных.");
                                }
                                else
                                {
                                    Debug.WriteLine("Ошибка при добавлении записи в базу данных.");
                                }
                            }
                        }
                        localChanges.Clear();
                        return true;
                    }
                    catch (OleDbException ex)
                    {
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

        public void MoveCars(Track destinationTrack, bool IsFirst = false)
        {
            List<Car> selectedCars = SelectedTrack.GetSelectedCars();
            List<Car> oldSelectedCars = new List<Car>();
            foreach (Car car in selectedCars) oldSelectedCars.Add(car.Clone());


            if (destinationTrack.Cars.Count + selectedCars.Count <= destinationTrack.Capacity)
            {
                SelectedTrack.RemoveAllCars(selectedCars);

                if (!IsFirst)
                {
                    destinationTrack.AddLast(selectedCars);
                }
                else
                {
                    destinationTrack.AddFirst(selectedCars);
                }
            }
            for (int i = 0; i < selectedCars.Count; i++)
            {
                localChanges.Add(new LocalChange(oldSelectedCars[i], selectedCars[i]));
            }
        }

        public void ConnectToDefectCodesDatabase()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionToDefectCodesDbString))
            {
                try
                {
                    var queryString = "SELECT * FROM DefectCodes";
                    OleDbCommand command = new OleDbCommand(queryString, connection);

                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string code = reader.GetString(1);
                        string fullName = reader.GetString(2);
                        string shortName = reader.GetString(4);
                        bool isPouring = reader.GetBoolean(3);

                        DefectCodes.Add(new DefectCode(code, fullName, shortName, isPouring));
                    }
                    reader.Close();
                }
                catch (OleDbException ex)
                {
                    Debug.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
        }

        public void ConnectToStationsDatabase()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionToStationsDbString))
            {
                try
                {
                    var getAllStationsQueryString = "SELECT * FROM Stations";
                    OleDbCommand getAllStationsСommand = new OleDbCommand(getAllStationsQueryString, connection);

                    connection.Open();
                    OleDbDataReader stationReader = getAllStationsСommand.ExecuteReader();

                    while (stationReader.Read())
                    {
                        int stationId = stationReader.GetInt32(stationReader.GetOrdinal("StationID"));
                        string stationName = stationReader.GetString(stationReader.GetOrdinal("StationName"));
                        Station station = new Station(stationId, stationName);
                        Stations.Add(station);
                    }
                    stationReader.Close();

                    foreach (Station station in Stations)
                    {
                        var getTracksFromStationQueryString = "SELECT * FROM Tracks WHERE StationID = " + station.StationId;

                        OleDbCommand getTracksFromStationСommand = new OleDbCommand(getTracksFromStationQueryString, connection);

                        OleDbDataReader trackReader = getTracksFromStationСommand.ExecuteReader();

                        while (trackReader.Read())
                        {
                            int trackId = trackReader.GetInt32(trackReader.GetOrdinal("TrackID_"));
                            int trackNumber = trackReader.GetInt32(trackReader.GetOrdinal("TrackNumber"));
                            int capacity = trackReader.GetInt32(trackReader.GetOrdinal("Capacity"));
                            int stationId = trackReader.GetInt32(trackReader.GetOrdinal("StationId"));
                            Track track = new Track(trackId, trackNumber, capacity, stationId);
                            station.AddTrack(track);
                        }
                        trackReader.Close();

                        foreach (Track track in station.Tracks) 
                        {
                            var getCarsFromTrackQueryString = "SELECT * FROM Cars WHERE TrackID = " + track.TrackId;

                            OleDbCommand getCarsFromTrackСommand = new OleDbCommand(getCarsFromTrackQueryString, connection);

                            OleDbDataReader carReader = getCarsFromTrackСommand.ExecuteReader();

                            while (carReader.Read()) 
                            {
                                string carNumber = carReader.GetString(carReader.GetOrdinal("CarNumber"));
                                int serialNumber = carReader.GetInt32(carReader.GetOrdinal("SerialNumber"));
                                bool isFixed = carReader.GetBoolean(carReader.GetOrdinal("IsFixed"));
                                string product = carReader.GetString(carReader.GetOrdinal("Product"));
                                bool isLoaded = carReader.GetBoolean(carReader.GetOrdinal("IsLoaded"));
                                string defectCodes = carReader.GetString(carReader.GetOrdinal("DefectCodes"));
                                string cargo = carReader.GetString(carReader.GetOrdinal("Cargo"));
                                DateTime arrival = carReader.GetDateTime(carReader.GetOrdinal("Arrival"));
                                int trackId = carReader.GetInt32(carReader.GetOrdinal("TrackID"));
                                Car car = new Car(carNumber, serialNumber, isFixed, defectCodes, isLoaded, product, cargo, arrival, trackId);
                                car.CarChanged += OnCarChanged;
                                track.AddLast(car);
                            }

                            track.Sort();
                        }
                    }

                }
                catch (OleDbException ex)
                {
                    Debug.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
        }

        public void AddCarsToDatabase()
        {
            try
            {
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\User\source\repos\AvaloniaApplication2\Stations.accdb;";

                var insertQueryString = "INSERT INTO CARS (CarNumber, SerialNumber, IsFixed, DefectCodes, IsLoaded, Product, Cargo, Arrival, TrackID) VALUES (@CarNumber, @SerialNumber, @IsFixed, @DefectCodes, @IsLoaded, @Product, @Cargo, @Arrival, @TrackID)";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    for (int i = 0; i < 8; i++)
                    {

                        using (OleDbCommand command = new OleDbCommand(insertQueryString, connection))
                        {
                            // Задание значений параметров
                            command.Parameters.AddWithValue("@CarNumber", "11111");
                            command.Parameters.Add("@SerialNumber", OleDbType.Integer).Value = i + 1;
                            command.Parameters.Add("@IsFixed", OleDbType.Boolean).Value = true;
                            command.Parameters.AddWithValue("@DefectCodes", "1");
                            command.Parameters.Add("@IsLoaded", OleDbType.Boolean).Value = false; 
                            command.Parameters.AddWithValue("@Product", "1");
                            command.Parameters.AddWithValue("@Cargo", "1");
                            command.Parameters.Add("@Arrival", OleDbType.DBDate).Value = DateTime.Now;
                            command.Parameters.Add("@TrackID", OleDbType.Integer).Value = 31;

                            // Выполнение запроса
                            int rowsAffected = command.ExecuteNonQuery();

                            // Проверка на количество затронутых строк (обычно 1)
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Запись успешно добавлена в базу данных.");
                            }
                            else
                            {
                                Console.WriteLine("Ошибка при добавлении записи в базу данных.");
                            }
                        }
                    }

                }
            }                   
            catch (OleDbException ex)
            {
                Debug.WriteLine("Error connecting to the database: " + ex.Message);
            }
        }


        public void UpdateCarNumbers()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionToStationsDbString))
            {
                try
                {
                    var queryString = "SELECT * FROM Cars";
                    OleDbCommand command = new OleDbCommand(queryString, connection);

                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int carId = reader.GetInt32(0);

                        UpdateCarNumber(connection, carId, 10000 + carId);
                    }
                    reader.Close();
                }
                catch (OleDbException ex)
                {
                    Debug.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
        }

        private void UpdateCarNumber(OleDbConnection connection, int carId, int newCarNumber)
        {
            string updateQuery = "UPDATE Cars SET CarNumber = @NewCarNumber WHERE Код = @CarId";
            OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@NewCarNumber", newCarNumber);
            updateCommand.Parameters.AddWithValue("@CarId", carId);
            try
            {
                int rowsAffected = updateCommand.ExecuteNonQuery();

                // Проверка на количество затронутых строк (обычно 1)
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Запись успешно добавлена в базу данных.");
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении записи в базу данных.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating car number: " + ex.Message);
            }
        }
    }
}
