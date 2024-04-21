using System.Globalization;

namespace NsDataTest
{
    public enum StopType
    {
        First,
        Passing,
        Short,
        Regular,
        Terminus
    }

    internal class Stop
    {
        public Station? Station { get; protected set; }
        public StopType StopType { get; protected set; }
    }

    internal class FirstStop : Stop 
    {
        public FirstStop(Station station, string departureTime, Track arrivalTrack, Track departureTrack, FootNote trackFootNote) 
        {
            Station = station;
            StopType = StopType.First;
            string hour = departureTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                departureTime = departureTime.Replace(hour, "00");
            }
            DepartureTime = TimeOnly.ParseExact(departureTime, "HHmm", CultureInfo.InvariantCulture);
            ArrivalTrack = arrivalTrack;
            DepartureTrack = departureTrack;
            TrackFootNote = trackFootNote;
        }

        public TimeOnly DepartureTime { get; private set; }
        public Track ArrivalTrack { get; private set; }
        public Track DepartureTrack { get; private set; }
        public FootNote TrackFootNote { get; private set; }

    }

    internal class TracklessFirstStop : Stop
    {
        public TracklessFirstStop(Station station, string departureTime)
        {
            Station = station;
            StopType = StopType.Terminus;
            string hour = departureTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                departureTime = departureTime.Replace(hour, "00");
            }
            DepartureTime = TimeOnly.ParseExact(departureTime, "HHmm", CultureInfo.InvariantCulture);
        }

        public TimeOnly DepartureTime { get; private set; }
    }

    internal class PassingStop : Stop
    {
        public PassingStop(Station station)
        {
            Station = station;
            StopType = StopType.Passing;
        }
    }

    internal class ShortStop : Stop 
    {
        public ShortStop(Station station, string arrivalAndDepartureWithinMinuteOf, Track arrivalTrack, Track departureTrack, FootNote trackFootNote)
        {
            Station = station;
            StopType = StopType.Short;
            string hour = arrivalAndDepartureWithinMinuteOf.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                arrivalAndDepartureWithinMinuteOf = arrivalAndDepartureWithinMinuteOf.Replace(hour, "00");
            }
            ArrivalAndDepartureWithinMinuteOf = TimeOnly.ParseExact(arrivalAndDepartureWithinMinuteOf, "HHmm", CultureInfo.InvariantCulture);
            ArrivalTrack = arrivalTrack;
            DepartureTrack = departureTrack;
            TrackFootNote = trackFootNote;
        }

        public TimeOnly ArrivalAndDepartureWithinMinuteOf { get; private set; }
        public Track ArrivalTrack { get; private set; }
        public Track DepartureTrack { get; private set; }
        public FootNote TrackFootNote { get; private set; }
    }

    internal class TracklessShortStop : Stop
    {
        public TracklessShortStop(Station station, string arrivalAndDepartureWithinMinuteOf)
        {
            Station = station;
            StopType = StopType.Short;
            string hour = arrivalAndDepartureWithinMinuteOf.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                arrivalAndDepartureWithinMinuteOf = arrivalAndDepartureWithinMinuteOf.Replace(hour, "00");
            }
            ArrivalAndDepartureWithinMinuteOf = TimeOnly.ParseExact(arrivalAndDepartureWithinMinuteOf, "HHmm", CultureInfo.InvariantCulture);
        }

        public TimeOnly ArrivalAndDepartureWithinMinuteOf { get; private set; }
    }

    internal class RegularStop : Stop
    {
        public RegularStop(Station station, string arrivalTime, string departureTime, Track arrivalTrack, Track departureTrack, FootNote trackFootNote)
        {
            Station = station;
            StopType = StopType.Regular;
            string hour = arrivalTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                arrivalTime = arrivalTime.Replace(hour, "00");
            }
            ArrivalTime = TimeOnly.ParseExact(arrivalTime, "HHmm", CultureInfo.InvariantCulture);
            hour = departureTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                departureTime = departureTime.Replace(hour, "00");
            }
            DepartureTime = TimeOnly.ParseExact(departureTime, "HHmm", CultureInfo.InvariantCulture); 
            ArrivalTrack = arrivalTrack;
            DepartureTrack = departureTrack;
            TrackFootNote = trackFootNote;
        }

        public TimeOnly ArrivalTime { get; private set; }
        public TimeOnly DepartureTime { get; private set; }
        public Track ArrivalTrack { get; private set; }
        public Track DepartureTrack { get; private set; }
        public FootNote TrackFootNote { get; private set; }
    }    

    internal class TracklessRegularStop : Stop
    {
        public TracklessRegularStop(Station station, string arrivalTime, string departureTime)
        {
            Station = station;
            StopType = StopType.Regular;
            string hour = arrivalTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                arrivalTime = arrivalTime.Replace(hour, "00");
            }
            ArrivalTime = TimeOnly.ParseExact(arrivalTime, "HHmm", CultureInfo.InvariantCulture);
            hour = departureTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                departureTime = departureTime.Replace(hour, "00");
            }
            DepartureTime = TimeOnly.ParseExact(departureTime, "HHmm", CultureInfo.InvariantCulture); 
        }

        public TimeOnly ArrivalTime { get; private set; }
        public TimeOnly DepartureTime { get; private set; }
    }

    internal class TerminusStop : Stop
    {
        public TerminusStop(Station station, string arrivalTime, Track arrivalTrack, Track departureTrack, FootNote trackFootNote)
        {
            Station = station;
            StopType = StopType.Terminus;
            string hour = arrivalTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                arrivalTime = arrivalTime.Replace(hour, "00");
            }
            ArrivalTime = TimeOnly.ParseExact(arrivalTime, "HHmm", CultureInfo.InvariantCulture);
            ArrivalTrack = arrivalTrack;
            DepartureTrack = departureTrack;
            TrackFootNote = trackFootNote;
        }

        public TimeOnly ArrivalTime { get; private set; }
        public Track ArrivalTrack { get; private set; }
        public Track DepartureTrack { get; private set; }
        public FootNote TrackFootNote { get; private set; }
    }

    internal class TracklessTerminusStop : Stop
    {
        public TracklessTerminusStop(Station station, string arrivalTime)
        {
            Station = station;
            StopType = StopType.Terminus;
            string hour = arrivalTime.Substring(0, 2);
            if (int.Parse(hour) > 23)
            {
                arrivalTime = arrivalTime.Replace(hour, "00");
            }
            ArrivalTime = TimeOnly.ParseExact(arrivalTime, "HHmm", CultureInfo.InvariantCulture);
        }

        public TimeOnly ArrivalTime { get; private set; }
    }    
}
