namespace NsDataTest
{
    internal class TrainJourney
    {
        public TrainJourney(uint id, Company company, string lineNmbr, ushort startStopNmbr, ushort endStopNmbr) 
        {
            Id = id;
            Company = company;
            LineNmbr = lineNmbr;
            StartStopNmbr = startStopNmbr;
            EndStopNmbr = endStopNmbr;
        }

        public uint Id {  get; private set; }

        public Company Company { get; private set; }
        public string LineNmbr { get; private set; }
        public ushort StartStopNmbr { get; private set; }
        public ushort EndStopNmbr { get; private set;}
    }
}
