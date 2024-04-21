using System.Text;

namespace NsDataTest
{
    internal class Station
    {
        public Station(
            bool isTransferstation, 
            string shortName, 
            ushort averageTransferTime, 
            ushort maximumTransferTime,
            Country country,
            Timezone timezone,
            int xLocation,
            int yLocation,
            string fullName) 
        {
            IsTranserStation = isTransferstation;
            ShortName = shortName;
            AverageTransferTime = averageTransferTime;
            MaximumTransferTime = maximumTransferTime;
            Country = country;
            Timezone = timezone;
            XLocation = xLocation; 
            YLocation = yLocation;
            FullName = fullName;
        }

        public bool IsTranserStation { get; private set; }
        public string ShortName { get; private set; }
        public ushort AverageTransferTime { get; private set; }
        public ushort MaximumTransferTime { get; private set; }
        public Country Country { get; private set; }
        public Timezone Timezone { get; private set; }
        public int XLocation { get; private set; }
        public int YLocation { get; private set; }
        public string FullName { get; private set; }

        private readonly static Dictionary<string, Station> _Stations = new Dictionary<string, Station>();
        static Station()
        {
            using (FileStream fs = File.OpenRead("Dataset/stations.dat"))
            using (StreamReader sr = new StreamReader(fs, Encoding.Latin1))
            {
                // Skip delivery line
                sr.ReadLine();
                while (sr.Peek() != -1)
                {
                    string? line = sr.ReadLine();
                    if (line != null)
                    {
                        string[] attributes = line.Split(',');
                        _Stations.Add(
                            attributes[1].Trim(),
                            new Station(
                                attributes[0].Trim() == "1",
                                attributes[1].Trim(),
                                ushort.Parse(attributes[2].Trim()),
                                ushort.Parse(attributes[3].Trim()),
                                Country.GetByCode(attributes[4].Trim()),
                                Timezone.GetById(ushort.Parse(attributes[5].Trim())),
                                // skip attributes[6] since as per the documentation, it is always empty.
                                int.Parse(attributes[7].Trim()),
                                int.Parse(attributes[8].Trim()),
                                attributes[9].Trim()
                        ));
                    }
                }
            }
        }

        public static Station GetByShortName(string shortName)
        {
            return _Stations[shortName];
        }
    }
}
