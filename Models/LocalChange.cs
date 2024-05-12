using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class LocalChange
    {
        private static readonly string _startTemplate = "UPDATE Cars SET ";
        private static readonly string _endTemplate = " WHERE CarNumber = ";

        private Car oldCar;
        private Car newCar;

        public LocalChange(Car oldCar, Car newCar)
        {
            this.oldCar = oldCar;
            this.newCar = newCar;
        }

        public string GetSQL()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(_startTemplate);

            if (oldCar.SerialNumber != newCar.SerialNumber)
                queryBuilder.Append($"SerialNumber = '{newCar.SerialNumber}', ");

            if (oldCar.DefectCodes != newCar.DefectCodes)
                queryBuilder.Append($"DefectCodes = '{newCar.DefectCodes}', ");

            if (oldCar.IsFixed != newCar.IsFixed)
                queryBuilder.Append($"IsFixed = {(newCar.IsFixed ? 1 : 0)}, ");

            if (oldCar.IsLoaded != newCar.IsLoaded)
                queryBuilder.Append($"IsLoaded = {(newCar.IsLoaded ? 1 : 0)}, ");
            
            if (oldCar.Product != newCar.Product)
                queryBuilder.Append($"Product = '{newCar.Product}', ");

            if (oldCar.Cargo != newCar.Cargo)
                queryBuilder.Append($"Cargo = '{newCar.Cargo}', ");

            if (oldCar.Arrival != newCar.Arrival)
                queryBuilder.Append($"Arrival = '{newCar.Arrival.ToString("yyyy-MM-dd HH:mm:ss")}', ");

            if (oldCar.TrackId != newCar.TrackId)
                queryBuilder.Append($"TrackID = '{newCar.TrackId}', ");

            queryBuilder.Length -= 2;

            queryBuilder.Append(_endTemplate);
            queryBuilder.Append("\'" + newCar.CarNumber.ToString() + "\'");

            return queryBuilder.ToString();
        }

        public bool HasSameCar(LocalChange change)
        {
            return oldCar.Equals(change.oldCar);
        }
    }
}
