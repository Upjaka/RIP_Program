using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class NewCar : INotifyPropertyChanged
    {
        private int _serialNumber;
        public int SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                if (value != _serialNumber)
                {
                    _serialNumber = value;
                    OnPropertyChanged(nameof(SerialNumber));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NewCar()
        {
            _carNumber = "";
            _isLoaded = false;
            _serialNumber = 1;
        }

        public NewCar(int serialNumber)
        {
            _carNumber = "";
            _isLoaded = false;
            _serialNumber = serialNumber;
        }

        public NewCar(int serialNumber, string carNumber)
        {
            _carNumber = carNumber;
            _isLoaded = false;
            _serialNumber = serialNumber;
        }
    }
}
