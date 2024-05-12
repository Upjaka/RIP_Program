using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class Car
    {
        public bool IsSelected { get; set; }
        public string CarNumber { get; set; }
        public int SerialNumber { get; set; }
        public string Product { get; set; }
        public string DefectCodes { get; set; }
        public string Cargo { get; set; }
        public bool IsFixed { get; set; }
        public bool IsLoaded { get; set; }
        public DateTime Arrival { get; set; }
        public int TrackId { get; set; }

        public Car(string carNumber, int serialNumber, bool isFixed, string defectCodes, bool isLoaded, string product, string cargo, DateTime arrival, int trackId)
        {
            IsSelected = false;

            CarNumber = carNumber;
            SerialNumber = serialNumber;
            DefectCodes = defectCodes;
            IsFixed = isFixed;
            Product = product;
            Cargo = cargo;
            IsLoaded = isLoaded;

            Arrival = arrival;
            TrackId = trackId;
        }

        public override string ToString()
        {
            return "№п/п " + SerialNumber + ", №вагона " + CarNumber + ", коды брака " + DefectCodes + ", продукт " + Product + ", груз " + Cargo + ", прибытие " + Arrival;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Car || obj == null)
            {
                return false;
            }
            else
            {
                Car other = obj as Car;
                return (SerialNumber == other.SerialNumber &&
                                    CarNumber == other.CarNumber &&
                                    IsFixed == other.IsFixed &&
                                    DefectCodes == other.DefectCodes &&
                                    Cargo == other.Cargo &&
                                    Product == other.Product &&
                                    Arrival == other.Arrival &&
                                    TrackId == other.TrackId);
            }
        }

        public Car Clone()
        {
            return new Car(
                CarNumber,
                SerialNumber,
                IsFixed,
                DefectCodes,
                IsLoaded,
                Product,
                Cargo,
                Arrival,
                TrackId
                );
        }
    }
}
