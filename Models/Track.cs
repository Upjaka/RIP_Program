using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class Track
    {
        public int TrackId { get; set; }
        public int TrackNumber { get; set; }
        public int Capacity { get; set; }
        public LinkedList<Car> Cars { get; set; } = new LinkedList<Car>();
        public int StationId { get; set; }
        public int TrackPosition { get; set; }

        private Comparison<Car> customComparison = (x, y) =>
        {
            int result = x.SerialNumber.CompareTo(y.SerialNumber);
            return result;
        };

        public Track()
        {
            TrackId = 0;
            TrackNumber = 0;
            Capacity = 0;
            StationId = 0;
            TrackPosition = 1;
        }

        public Track(int trackId, int trackNumber, int capacity, int stationId, int trackPosition) 
        {
            TrackId = trackId;
            TrackNumber = trackNumber;
            Capacity = capacity;
            StationId = stationId;
            TrackPosition = trackPosition;
        }

        public Car? GetCar(int serialNumber) 
        { 
            foreach (var car in Cars)
            {
                if (car.SerialNumber == serialNumber)
                {
                    return car;
                }
            }
            return null;
        }

        public void AddLast(Car car)
        {
            Cars.AddLast(car);
            car.TrackId = TrackId;
        }

        public void AddLast(List<Car> cars) 
        {
            int lastIndex = (Cars.Count == 0) ? 1 : Cars.Count + 1;
            foreach (var car in cars)
            {
                car.SerialNumber = lastIndex++;
                Cars.AddLast(car);
            }
        }

        public void AddFirst(List<Car> cars)
        {
            int serialNumber = 1;

            foreach (Car car in cars)
            {
                car.SerialNumber = serialNumber++;
                car.TrackId = TrackId;
            }
            foreach (Car car in Cars)
            {
                car.SerialNumber = serialNumber++;
            }
            cars.Reverse();
            foreach (Car car in cars)
            {
                Cars.AddFirst(car);
            }
        }

        public void RemoveCar(Car car)
        {            
            if (car != Cars.Last.Value)
            {
                Cars.Remove(car);

                int i = 1;
                foreach (Car car1 in Cars)
                {
                    car1.SerialNumber = i++;
                }
            } else
            {
                Cars.Remove(car);
            }            
        }

        public void RemoveAllCars(List<Car> cars)
        {
            foreach (Car car in cars) {
                Cars.Remove(car);
            }

            int i = 1;
            foreach (Car car in Cars)
            {
                car.SerialNumber = i++;
            }
        }

        public List<Car> GetSelectedCars()
        {
            var result = new List<Car>();

            foreach (var car in Cars)
            {
                if (car.IsSelected)
                {
                    result.Add(car);
                }
            }
            return result;
        }

        public void Sort()
        {
            var list = new List<Car>(Cars);

            list.Sort(customComparison);
            Cars.Clear();
            foreach (var car in list)
            {
                Cars.AddLast(car);
            }
        }

        public override string ToString()
        {
            return "ID: " + TrackId + ", Number: " + TrackNumber + ", Capacity: " + Capacity + ", CarCount: " + Cars.Count;
        }
    }
}
