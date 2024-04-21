using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace NsDataTest
{
    internal struct Track
    {
        public Track(string trackString)
        {
            TrackString = trackString;

/*            if (trackString.Length > 1) 
            {
                Regex re = new Regex(@"\d+");
                Number = uint.Parse(re.Match(trackString).Value);
                TrackDivision = trackString.Replace(Number.ToString(), "");
            }*/
        }

        public uint Number { get; private set; }
        public string? TrackDivision { get; private set; }
        public string TrackString { get; private set; }
    }
}
