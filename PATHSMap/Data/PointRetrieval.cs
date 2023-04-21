using System;
using System.Net.NetworkInformation;

namespace PATHSMap.Data
{
    public class PointRetrieval
    {
        public PointRetrieval()
        {
        }
        //this method is used to form coordinate list into tuple couples.
        public static List<Tuple<double, double>> CoordinateRetrieval(List<double> coords)
        {
            if (coords == null || coords.Count % 2 != 0)
            {
                throw new ArgumentException("Invalid list of coordinates.");
            }
            List<Tuple<double, double>> coordPairs = new List<Tuple<double, double>>();
            for (int i = 0; i < coords.Count; i += 2)
            {
                double lat = coords[i];
                double lon = coords[i + 1];
                Tuple<double, double> coordPair = Tuple.Create(lon, lat);
                coordPairs.Add(coordPair);
            }
            return coordPairs;
        }
    }
}

