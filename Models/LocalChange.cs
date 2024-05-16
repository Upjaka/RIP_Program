using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class LocalChange
    {
        private static readonly string _UpdateStartTemplate = "UPDATE Cars SET ";
        private static readonly string _UpdateEndTemplate = " WHERE CarNumber = ";

        private static readonly string _InsertStartTemplate = "INSERT INTO Cars (CarNumber, SerialNumber, IsFixed, DefectCodes, IsLoaded, Product, Cargo, Arrival, TrackID) VALUES (";

        private Car? oldCar;
        private Car newCar;

        private bool _isInsert;
        public bool IsInsert { get { return _isInsert; } }

        public LocalChange(Car oldCar, Car newCar)
        {
            this.oldCar = oldCar;
            this.newCar = newCar;

            _isInsert = false;
        }

        public LocalChange(Car newCar)
        {
            oldCar = newCar;
            this.newCar= newCar;

            _isInsert= true;
        }

        public string GetSQL()
        {
            StringBuilder queryBuilder = new StringBuilder();

            if (IsInsert)
            {
                queryBuilder.Append(_InsertStartTemplate);

                queryBuilder.Append($"'{newCar.CarNumber}', ");
                queryBuilder.Append($"'{newCar.SerialNumber}', ");
                queryBuilder.Append($"{(newCar.IsFixed ? 1 : 0)}, ");
                queryBuilder.Append($"'{newCar.DefectCodes}', ");
                queryBuilder.Append($"{(newCar.IsLoaded ? 1 : 0)}, ");
                queryBuilder.Append($"'{newCar.Product}', ");
                queryBuilder.Append($"'{newCar.Cargo}', ");
                queryBuilder.Append($"'{newCar.Arrival.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                queryBuilder.Append($"'{newCar.TrackId}')");
            }
            else
            {
                queryBuilder.Append(_UpdateStartTemplate);

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

                queryBuilder.Append(_UpdateEndTemplate);
                queryBuilder.Append("\'" + newCar.CarNumber.ToString() + "\'");
            }
            return queryBuilder.ToString();
        }

        public bool HasSameCar(LocalChange change)
        {
            return oldCar.Equals(change.oldCar);
        }

        public void SetNewCar(LocalChange change)
        {
            newCar = change.newCar;
        }
    }
}
