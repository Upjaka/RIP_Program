using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class CarInfo
    {
        public string CarNumber { get; set; }
        public string PPNumber { get; set; }
        public bool IsFixed { get; set; }
        public string DefectCodes { get; set; }
        public bool IsLoaded { get; set; }
        public string Product { get; set; }
        public string Cargo { get; set; }

        public CarInfo(string ppNumber, string carNumber, bool isFixed, string defectCodes, bool isLoaded, string product, string cargo)
        {
            CarNumber = carNumber;
            PPNumber = ppNumber;
            IsFixed = isFixed;
            DefectCodes = defectCodes;
            IsLoaded = isLoaded;
            Product = product;
            Cargo = cargo;
        }

        public override string ToString()
        {
            return "№п/п " + PPNumber + ", №вагона " + CarNumber + ", коды брака " + DefectCodes + ", продукт " + Product + ", груз " + Cargo;
        }
    }
}
