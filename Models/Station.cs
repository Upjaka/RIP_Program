using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class Station
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public List<Track> Tracks { get; set; } = new List<Track>();    

        public Station(int stationId, string stationName) 
        { 
            StationId = stationId;
            StationName = stationName;
        }

        public void AddTrack(Track track) { Tracks.Add(track); }

        public void RemoveTrack(Track track) { Tracks.Remove(track); }

        public void AddAllTrack(List<Track> tracks) { Tracks.AddRange(tracks); }

        public Track GetTrackById(int id) { return Tracks[id]; }

        public override string ToString()
        {
            return "ID: "+ StationId + ", Name: " + StationName + ", TrackCount: " + Tracks.Count;
        }
    }
}
