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

        public Car(string carNumber, int serialNumber, bool isFixed, string defectCodes, bool isLoaded, string product, string cargo, DateTime arrival)
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
        }

        public override string ToString()
        {
            return "№п/п " + SerialNumber + ", №вагона " + CarNumber + ", коды брака " + DefectCodes + ", продукт " + Product + ", груз " + Cargo + ", прибытие " + Arrival;
        }
    }
}
