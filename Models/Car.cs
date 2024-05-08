using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class Car
    {
        public string CarNumber { get; set; }
        public int SerialNumber { get; set; }
        public string Product { get; set; }
        public string DefectCodes { get; set; }
        public string Cargo { get; set; }
        public DateTime Arrival { get; set; }

        public Car(string carNumber, int serialNumber, string defectCodes, string product, string cargo, DateTime arrival)
        {
            CarNumber = carNumber;
            SerialNumber = serialNumber;
            DefectCodes = defectCodes;
            Product = product;
            Cargo = cargo;
            Arrival = arrival;
        }

        public override string ToString()
        {
            return "№п/п " + SerialNumber + ", №вагона " + CarNumber + ", коды брака " + DefectCodes + ", продукт " + Product + ", груз " + Cargo + ", прибытие " + Arrival;
        }
    }
}
