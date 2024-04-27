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

            ConnectToDatabase();
        }

        public void ConnectToDatabase()
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

                        //Debug.WriteLine($"Code: {code}, FullName: {fullName}, ShortName: {shortName}, IsPouring: {isPouring}");
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
    }
}
