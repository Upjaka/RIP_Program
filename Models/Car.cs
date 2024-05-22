using System;
using System.ComponentModel;

namespace AvaloniaApplication2.Models
{
    public class Car : INotifyPropertyChanged
    {
        public bool IsSelected { get; set; }
        public string CarNumber { get; }
        private int _serialNumber;
        public int SerialNumber
        {
            get => _serialNumber;
            set
            {
                if (_serialNumber != value)
                {
                    Car oldCar = Clone();
                    _serialNumber = value;
                    OnCarChanged(oldCar, this);
                }
            }
        }
        private string _product;
        public string Product
        {
            get => _product;
            set
            {
                if (_product != value)
                {
                    Car oldCar = Clone();
                    _product = value;
                    OnCarChanged(oldCar, this);
                }
            }
        }
        private string _defectCodes;
        public string DefectCodes
        {
            get => _defectCodes;
            set
            {
                if (_defectCodes != value)
                {
                    Car oldCar = Clone();
                    _defectCodes = value;
                    OnCarChanged(oldCar, this);
                }
            }
        }
        private string _cargo;
        public string Cargo
        {
            get => _cargo;
            set
            {
                if (_cargo != value)
                {
                    Car oldCar = Clone();
                    _cargo = value;
                    OnCarChanged(oldCar, this);
                }
            }
        }
        private bool _isFixed;
        public bool IsFixed
        {
            get => _isFixed;
            set
            {
                if (_isFixed != value)
                {
                    Car oldCar = Clone();
                    _isFixed = value;
                    OnCarChanged(oldCar, this);
                }
            }
        }
        private bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                if (_isLoaded != value)
                {
                    Car oldCar = Clone();
                    _isLoaded = value;
                    OnCarChanged(oldCar, this);
                }
            }
        }
        public DateTime Arrival { get; set; }
        private int _trackId;
        public int TrackId
        {
            get => _trackId;
            set
            {
                if (_trackId != value)
                {
                    Car oldCar = Clone();
                    _trackId = value;
                    OnCarChanged(oldCar, this);
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<CarChangedEventArgs> CarChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnCarChanged(Car oldCar, Car newCar)
        {
            CarChanged?.Invoke(this, new CarChangedEventArgs(oldCar, newCar));
        }
    }

    public class CarChangedEventArgs : EventArgs
    {
        public Car OldCar { get; }
        public Car NewCar { get; }

        public CarChangedEventArgs(Car oldCar, Car newCar)
        {
            OldCar = oldCar;
            NewCar = newCar;
        }
    }
}
