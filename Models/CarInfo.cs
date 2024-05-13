using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class CarInfo : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(_isSelected));
                }
            }
        }
        private string _carNumber;
        public string CarNumber
        {
            get { return _carNumber; }
            set
            {
                if (_carNumber != value)
                {
                    _carNumber = value;
                    OnPropertyChanged(nameof(_carNumber));
                }
            }
        }
        private string _serialNumber;
        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                if (_serialNumber != value)
                {
                    _serialNumber = value;
                    OnPropertyChanged(nameof(_serialNumber));
                }
            }
        }
        private bool _isFixed;
        public bool IsFixed
        {
            get { return _isFixed; }
            set
            {
                if (_isFixed != value)
                {
                    _isFixed = value;
                    OnPropertyChanged(nameof(_isFixed));
                }
            }
        }
        private string _defectCodes;
        public string DefectCodes
        {
            get { return _defectCodes; }
            set
            {
                if (_defectCodes != value)
                {
                    _defectCodes = value;
                    OnPropertyChanged(nameof(_defectCodes));
                }
            }
        }
        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    OnPropertyChanged(nameof(_isLoaded));
                }
            }
        }
        private string _product;
        public string Product
        {
            get { return _product; }
            set
            {
                if (_product != value)
                {
                    _product = value;
                    OnPropertyChanged(nameof(_product));
                }
            }
        }
        private string _cargo;
        public string Cargo
        {
            get { return _cargo; }
            set
            {
                if (_cargo != value)
                {
                    _cargo = value;
                    OnPropertyChanged(nameof(_cargo));
                }
            }
        }

        public CarInfo(Car car) 
        {
            _isSelected = car.IsSelected;
            _carNumber = car.CarNumber;
            _serialNumber = car.SerialNumber.ToString();
            _isFixed = car.IsFixed;
            _defectCodes = car.DefectCodes;
            _isLoaded = car.IsLoaded;
            _product = car.Product;
            _cargo = car.Cargo;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return "№п/п " + SerialNumber + ", №вагона " + CarNumber + ", коды брака " + DefectCodes + ", продукт " + Product + ", груз " + Cargo;
        }

        public override bool Equals(object obj) 
        {
            if (obj is not CarInfo)
            {
                return false;
            }
            else
            {
                CarInfo other = (CarInfo)obj;
                if (other.CarNumber == CarNumber &&
                    other.SerialNumber == SerialNumber && 
                    other.IsFixed == IsFixed &&
                    other.DefectCodes == DefectCodes &&
                    other.Cargo == Cargo &&
                    other.Product == Product &&
                    other.IsLoaded == IsLoaded) return true;
                else return false;
            }
        }
    }
}