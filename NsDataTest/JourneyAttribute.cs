using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NsDataTest
{
    internal class JourneyAttribute
    {
        public JourneyAttribute(Attribute attribute, ushort startStop, ushort endStop, FootNote footNote)
        {
            Attribute = attribute;
            StartStop = startStop;
            EndStop = endStop;
            FootNote = footNote;
        }

        public Attribute Attribute { get; private set; }
        public ushort StartStop { get; private set; }
        public ushort EndStop { get; private set; }
        public FootNote FootNote { get; private set; }
        
    }
}
