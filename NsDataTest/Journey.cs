using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace NsDataTest
{    
    internal class Journey
    {
        private Journey(
            uint id, 
            List<TrainJourney> trainJourneys, 
            FootNote footNote, 
            ushort footNoteStartStop, 
            ushort footNoteStopStop,
            List<TransitType> transitTypes,
            List<JourneyAttribute> journeyAttributes, 
            List<Stop> stops) 
        {
            Id = id;
            TrainJourneys = trainJourneys;
            FootNote = footNote;
            FootNoteStartStop = footNoteStartStop;
            FootNoteStopStop = footNoteStopStop;
            TransitTypes = transitTypes;
            Attributes = journeyAttributes;
            Stops = stops;
        }

        public string GenerateItinerary()
        {
            return GenerateItinerary(this);
        }

        public uint Id { get; private set; }
        public List<TrainJourney> TrainJourneys { get; private set; }
        public FootNote FootNote { get; private set; }
        public ushort FootNoteStartStop { get; private set; }
        public ushort FootNoteStopStop { get; private set; }
        public List<TransitType> TransitTypes { get; private set; }
        public List<JourneyAttribute> Attributes { get; private set; }
        public List<Stop> Stops { get; private set; } 

        private readonly static Dictionary<uint, Journey> _Journeys 
            = new Dictionary<uint, Journey>();
        public static Dictionary<uint, Journey> AllJourneys 
        { 
            get { return _Journeys; } 
        }
        static Journey()
        {
            using (FileStream fs = File.OpenRead("Dataset/timetbls.dat"))
            using (StreamReader sr = new StreamReader(fs, Encoding.Latin1))
            {              
                // Skip delivery line
                sr.ReadLine();

                string? line = sr.ReadLine() ?? throw new Exception();
                while (sr.Peek() != -1)
                {
                    uint temp_Id = 0;
                    List<TrainJourney> temp_TrainJourneys = new List<TrainJourney>();
                    FootNote? temp_footNote = null;
                    ushort temp_footNoteStartStop = 0;
                    ushort temp_footNoteStopStop = 0;
                    List<TransitType> temp_TransitTypes = new List<TransitType>();
                    List<JourneyAttribute> temp_Attributes = new List<JourneyAttribute>();
                    List<Stop> temp_Stops = new List<Stop>();

                    if (line != null)
                    {
                        while (line.StartsWith('#'))
                        {
                            temp_Id = 0;
                            temp_TrainJourneys = new List<TrainJourney>();
                            temp_footNote = null;
                            temp_footNoteStartStop = 0;
                            temp_footNoteStopStop = 0;
                            temp_TransitTypes = new List<TransitType>();
                            temp_Attributes = new List<JourneyAttribute>();
                            temp_Stops = new List<Stop>();

                            // Start by getting the Id of the Journey
                            temp_Id = uint.Parse(line.Trim().Replace("#", ""));

                            // Get all of the TrainJourneys that make up this Journey
                            line = sr.ReadLine() ?? throw new Exception();
                            while (line.StartsWith('%'))
                            {
                                string[] attributes = line.Split(',');
                                temp_TrainJourneys.Add(new TrainJourney(
                                    uint.Parse(attributes[1].Trim()),
                                    Company.GetById(ushort.Parse(attributes[0].Trim().Replace("%", ""))),
                                    attributes[2].Trim(),
                                    ushort.Parse(attributes[3].Trim()),
                                    ushort.Parse(attributes[4].Trim())
                                    ));

                                line = sr.ReadLine() ?? throw new Exception();
                            }

                            // Get the FootNote(s)
                            while (line.StartsWith('-'))
                            {
                                string[] attributes = line.Split(',');
                                temp_footNote = FootNote.GetById(ushort.Parse(attributes[0].Trim().Replace("-", "")));
                                temp_footNoteStartStop = ushort.Parse(attributes[1].Trim());
                                temp_footNoteStopStop = ushort.Parse(attributes[2].Trim());

                                line = sr.ReadLine() ?? throw new Exception();
                            }

                            // Get the TransitType
                            while (line.StartsWith('&'))
                            {
                                string[] attributes = line.Split(',');
                                temp_TransitTypes.Add(TransitType.GetByCode(attributes[0].Trim().Replace("&", "")));

                                line = sr.ReadLine() ?? throw new Exception();
                            }

                            while (line.StartsWith('*'))
                            {
                                string[] attributes = line.Split(',');
                                temp_Attributes.Add(new JourneyAttribute(
                                    Attribute.GetByCode(attributes[0].Trim().Replace("*", "")),
                                    ushort.Parse(attributes[1].Trim()),
                                    ushort.Parse(attributes[2].Trim()),
                                    FootNote.GetById(ushort.Parse(attributes[3].Trim()))));

                                line = sr.ReadLine() ?? throw new Exception();
                            }

                            // Build up all of the stops
                            if (line.StartsWith(">"))
                            {
                                Station temp_station;
                                string temp_ArrivalTime;
                                string temp_DepartureTime;

                                // Start by adding the First stop
                                string[] attributes = line.Split(',');
                                temp_station = Station.GetByShortName(attributes[0].Trim().Replace(">", ""));
                                temp_DepartureTime = attributes[1].Trim();
                                line = sr.ReadLine() ?? throw new Exception();
                                if (line.StartsWith("?"))
                                {
                                    attributes = line.Split(',');
                                    temp_Stops.Add(new FirstStop(
                                        temp_station,
                                        temp_DepartureTime,
                                        new Track(attributes[0].Trim().Replace("?", "")),
                                        new Track(attributes[1].Trim()),
                                        FootNote.GetById(ushort.Parse(attributes[2].Trim()))
                                    ));

                                    line = sr.ReadLine() ?? throw new Exception();
                                }
                                else
                                    temp_Stops.Add(new TracklessFirstStop(
                                        temp_station,
                                        temp_DepartureTime
                                    ));

                                // Add the other stops until the Terminus stop has been reached
                                while (!line.StartsWith("<"))
                                {
                                    // Add any Passing stops 
                                    while (line.StartsWith(";"))
                                    {
                                        temp_Stops.Add(new PassingStop(Station.GetByShortName(line.Trim().Replace(";", ""))));

                                        line = sr.ReadLine(); if (line == null) throw new Exception();
                                    }

                                    // Add any Short stops
                                    if (line.StartsWith("."))
                                    {
                                        attributes = line.Split(',');
                                        temp_station = Station.GetByShortName(attributes[0].Trim().Replace(".", ""));
                                        temp_DepartureTime = attributes[1].Trim();
                                        line = sr.ReadLine(); if (line == null) throw new Exception();
                                        if (line.StartsWith("?"))
                                        {
                                            attributes = line.Split(',');
                                            temp_Stops.Add(new ShortStop(
                                                temp_station,
                                                temp_DepartureTime,
                                                new Track(attributes[0].Trim().Replace("?", "")),
                                                new Track(attributes[1].Trim()),
                                                FootNote.GetById(ushort.Parse(attributes[2].Trim()))
                                            ));

                                            line = sr.ReadLine(); if (line == null) throw new Exception();
                                        }
                                        else
                                            temp_Stops.Add(new TracklessShortStop(
                                                temp_station,
                                                temp_DepartureTime
                                            ));


                                    }

                                    // Add any Regular stops
                                    if (line.StartsWith("+"))
                                    {
                                        attributes = line.Split(',');
                                        temp_station = Station.GetByShortName(attributes[0].Trim().Replace("+", ""));
                                        temp_ArrivalTime = attributes[1].Trim();
                                        temp_DepartureTime = attributes[2].Trim();
                                        line = sr.ReadLine() ?? throw new Exception();   
                                        if (line.StartsWith("?"))
                                        {
                                            attributes = line.Split(',');
                                            temp_Stops.Add(new RegularStop(
                                                temp_station,
                                                temp_ArrivalTime,
                                                temp_DepartureTime,
                                                new Track(attributes[0].Trim().Replace("?", "")),
                                                new Track(attributes[1].Trim()),
                                                FootNote.GetById(ushort.Parse(attributes[2].Trim()))
                                            ));

                                            line = sr.ReadLine() ?? throw new Exception();
                                        }
                                        else
                                            temp_Stops.Add(new TracklessRegularStop(
                                                temp_station,
                                                temp_ArrivalTime,
                                                temp_DepartureTime
                                            ));
                                    }
                                }

                                // Add the terminus stop
                                if (line.StartsWith("<"))
                                {
                                    attributes = line.Split(',');
                                    temp_station = Station.GetByShortName(attributes[0].Trim().Replace("<", ""));
                                    temp_ArrivalTime = attributes[1].Trim();
                                    line = sr.ReadLine(); if (line == null) break;
                                    if (line.StartsWith("?"))
                                    {
                                        attributes = line.Split(',');
                                        temp_Stops.Add(new TerminusStop(
                                            temp_station,
                                            temp_ArrivalTime,
                                            new Track(attributes[0].Trim().Replace("?", "")),
                                            new Track(attributes[1].Trim()),
                                            FootNote.GetById(ushort.Parse(attributes[2].Trim()))
                                        ));

                                        line = sr.ReadLine() ?? throw new Exception();
                                    }
                                    else
                                        temp_Stops.Add(new TracklessTerminusStop(
                                            temp_station,
                                            temp_ArrivalTime
                                        ));
                                }
                            }
                            if (temp_footNote != null)
                                _Journeys.Add(
                                    temp_Id,
                                    new Journey(
                                        temp_Id,
                                        temp_TrainJourneys,
                                        temp_footNote,
                                        temp_footNoteStartStop,
                                        temp_footNoteStopStop,
                                        temp_TransitTypes,
                                        temp_Attributes,
                                        temp_Stops
                                        ));
                        }
                    }
                }
            }
        }

        public static Journey GetById(uint id)
        {
            return _Journeys[id];
        }

        public static bool TryGetJourneysFromInitialDepartureTime(
            TimeOnly time, out List<Journey> journeys, DateOnly? onDate = null)
        {
            var _foundJourneys = new List<Journey>();

            foreach (var j in  _Journeys)
            {
                var departureStop = j.Value.Stops[0] as FirstStop;
                if (departureStop?.DepartureTime == time)
                    if (onDate != null)
                        if (j.Value.FootNote.Validity[onDate.Value])
                            _foundJourneys.Add(j.Value);
                        else { }
                    else
                        _foundJourneys.Add(j.Value);
            }

            journeys = _foundJourneys;

            return _foundJourneys.Count > 1;
        }

        public static bool TryGetJourneysFromDepartureTime(
            TimeOnly time, out Dictionary<Stop, Journey> departures, DateOnly? onDate = null)
        {
            var _foundDepartures = new Dictionary<Stop, Journey>();

            foreach (var j in _Journeys)
            {
                foreach(Stop s in j.Value.Stops)
                {
                    switch (s)
                    {
                        case FirstStop _fs:
                            if (_fs?.DepartureTime == time)
                                if (onDate != null)
                                    if (j.Value.FootNote.Validity[onDate.Value])
                                        _foundDepartures.Add(s, j.Value);
                                    else { }
                                else
                                    _foundDepartures.Add(s, j.Value);
                            break;
                        case RegularStop _rs:
                            if (_rs?.DepartureTime == time)
                                if (onDate != null)
                                    if (j.Value.FootNote.Validity[onDate.Value])
                                        _foundDepartures.Add(s, j.Value);
                                    else { }
                                else
                                    _foundDepartures.Add(s, j.Value);
                            break;
                    }
                }
            }

            departures = _foundDepartures;
            if (_foundDepartures.Count > 1)
                return true;
            return false;
        }

        public static string GenerateItinerary(uint id)
        {
            if (id <= 0 && id <= _Journeys.Count) return $"Journey with id {id} does not exist, id has to be between 0 and {_Journeys.Count}";

            return GenerateItinerary(GetById(id));
        }
        public static string GenerateItinerary (Journey journey)
        {
            string s = $"\nJourney for {journey.TrainJourneys[0].Company.Name.Trim()}: " +
                $"{journey.TransitTypes[0].Description} [{journey.TransitTypes[0].Code}]\n";

            if (journey.Attributes.Count > 0)
            {
                s += "This journey has the following attributes: ";
                foreach (JourneyAttribute attribute in journey.Attributes)
                    s += $"From {journey.Stops[attribute.StartStop].Station?.FullName} " +
                        $"until {journey.Stops[attribute.EndStop].Station?.FullName}: \n" +
                        $" > {attribute.Attribute.Description}";
                s += "\n";
            }

            foreach (Stop stop in journey.Stops)
            {
                if (stop is not FirstStop and not TracklessFirstStop)
                    s += "\n|";
                switch (stop)
                {
                    case FirstStop _stop:
                        // Example output: / 13:34 - Dordrecht (track: 2)
                        s += $"\n / {_stop.DepartureTime} - {_stop.Station?.FullName} " +
                            $"(track: {_stop.DepartureTrack.TrackString})";
                        break;
                    case TracklessFirstStop _stop:
                        // Example output: 
                        //  / 15:35 - Hertogenbosch ('s) (track unavailable)
                        s += $"\n / {_stop.DepartureTime} - {_stop.Station?.FullName} " +
                            $"(track unavailable)";
                        break;
                    case ShortStop _stop:
                        // Example output:
                        //  > 13:56 - Schiedam Centrum (track: 5)
                        s += $"\n > {_stop.ArrivalAndDepartureWithinMinuteOf} - {_stop.Station?.FullName} " +
                            $"(track: {_stop.DepartureTrack.TrackString})";
                        break;
                    case TracklessShortStop _stop:
                        s += $"\n > {_stop.ArrivalAndDepartureWithinMinuteOf} - {_stop.Station?.FullName} " +
                            $"(track unavailable)";
                        break;
                    case PassingStop _stop:
                        // Example output:
                        //  | Zwijndrecht
                        s += $"\n| {_stop.Station?.FullName}";
                        break;
                    case RegularStop _stop:
                        // Example output:
                        // \ 13:48 (track: 8
                        //  | Rotterdam Centraal
                        // / 13:51 (track: 8)
                        s += $"\n \\ {_stop.ArrivalTime} (track: {_stop.ArrivalTrack.TrackString})" +
                            $"\n  | {_stop.Station?.FullName} " +
                            $"\n / {_stop.DepartureTime} (track: {_stop.DepartureTrack.TrackString})";
                        break;
                    case TracklessRegularStop _stop:
                        // Example output:
                        // \ 16:15 (track unavailable)
                        //  | Tilburg
                        // / 16:15 (track unavailable)
                        s += $"\n \\ {_stop.ArrivalTime} (track unavailable) " +
                            $"\n  | {_stop.Station?.FullName} " +
                            $"\n / {_stop.DepartureTime} (track unavailable)";
                        break;
                    case TerminusStop _stop:
                        // Example output
                        //  \ 15:39 - Lelystad Centrum (track: 2)
                        s += $"\n \\ {_stop.ArrivalTime} - {_stop.Station?.FullName} " +
                            $"(track: {_stop.ArrivalTrack.TrackString})";
                        break;
                    case TracklessTerminusStop _stop:
                        // Example output
                        //  \ 16:52 - Breda (track unavailable)
                        s += $"\n \\ {_stop.ArrivalTime} - {_stop.Station?.FullName} " +
                            $"(track unavailable)";
                        break;
                }
                if (stop is not TerminusStop and not TracklessTerminusStop)
                    s += "\n|";
            }

            return s;
        }
    }
}
