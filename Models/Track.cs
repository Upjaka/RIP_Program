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

        public Track(int trackId, int trackNumber, int capacity) 
        {
            TrackId = trackId;
            TrackNumber = trackNumber;
            Capacity = capacity;
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

        public void AddCar(Car car)
        {
            Cars.AddLast(car);
        }

        public void AddAllCars(List<Car> cars) 
        {
            foreach (var car in cars) Cars.AddLast(car);
        }

        public void RemoveCar(Car car)
        {
            Cars.Remove(car);
        }

        public void RemoveAllCars(List<Car> cars)
        {
            foreach (Car car in Cars) {
                Cars.Remove(car);
            }
        }

        public override string ToString()
        {
            return "ID: " + TrackId + ", Number: " + TrackNumber + ", Capacity: " + Capacity + ", CarCount: " + Cars.Count;
        }
    }
}
