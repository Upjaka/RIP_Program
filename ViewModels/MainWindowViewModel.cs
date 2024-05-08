using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using AvaloniaApplication2.Models;
using System.Diagnostics;

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
        public ObservableCollection<DefectCode> DefectCodes { get; }
        public List<Station> Stations { get; } = new List<Station>();
    
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

            DefectCodes = new ObservableCollection<DefectCode>();

            ConnectToDefectCodesDatabase();

            ConnectToStationsDatabase();
        }

        public void ConnectToDefectCodesDatabase()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\User\source\repos\AvaloniaApplication2\DefectCodes.accdb;";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
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
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\User\source\repos\AvaloniaApplication2\Stations.accdb;";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
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
                            Track track = new Track(trackId, trackNumber, capacity);
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
                                string product = carReader.GetString(carReader.GetOrdinal("Product"));
                                string defectCodes = carReader.GetString(carReader.GetOrdinal("DefectCodes"));
                                string cargo = carReader.GetString(carReader.GetOrdinal("Cargo"));
                                DateTime arrival = carReader.GetDateTime(carReader.GetOrdinal("Arrival"));
                                Car car = new Car(carNumber, serialNumber, product, defectCodes, cargo, arrival);
                                track.AddCar(car);
                            }
                        }
                    }

                }
                catch (OleDbException ex)
                {
                    Debug.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
        }
    }
}
