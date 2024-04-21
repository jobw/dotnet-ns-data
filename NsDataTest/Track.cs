namespace NsDataTest
{
    internal struct Track
    {
        public Track(string trackString)
        {
            TrackString = trackString;
        }

        public string TrackString { get; private set; }
    }
}
