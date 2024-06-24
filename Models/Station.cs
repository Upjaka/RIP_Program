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

        private Comparison<Track> customComparison = (x, y) =>
        {
            int result = x.TrackPosition.CompareTo(y.TrackPosition);
            return result;
        };

        public Station()
        {
            StationId = 0;
            StationName = string.Empty;
        }

        public Station(int stationId, string stationName) 
        { 
            StationId = stationId;
            StationName = stationName;
        }

        public void AddTrack(Track track) { Tracks.Add(track); }


        public void RemoveTrack(Track track) { Tracks.Remove(track); }

        public void AddTracks(List<Track> tracks) { Tracks.AddRange(tracks); }

        public Track? GetTrackByNumber(int n) {
            foreach (Track track in Tracks)
            {
                if (track.TrackNumber == n) return track;
            }
            return null;
        }

        public void Sort()
        {
            Tracks.Sort(customComparison);
        }

        public override string ToString()
        {
            return "ID: "+ StationId + ", Name: " + StationName + ", TrackCount: " + Tracks.Count;
        }
    }
}
