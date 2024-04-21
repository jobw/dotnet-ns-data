using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NsDataTest
{
    internal class Timezone
    {
        public Timezone(ushort id, short difference, DateOnly startDate, DateOnly endDate) 
        {
            Id = id;
            Difference = difference;
            StartDate = startDate;
            EndDate = endDate;
        }

        public ushort Id { get; private set; }
        public short Difference { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set;}

        private readonly static Dictionary<ushort, Timezone> _Timezones = new Dictionary<ushort, Timezone>();

        static Timezone()
        {
            using (FileStream fs = File.OpenRead("Dataset/timezone.dat"))
            using (StreamReader sr = new StreamReader(fs, Encoding.Latin1))
            {
                // Skip delivery line
                sr.ReadLine();
                while (sr.Peek() != -1)
                {
                    string? id = sr.ReadLine();
                    if (id != null)
                    {
                        id = id.Trim().Replace("#", "");
                        string? line = sr.ReadLine();
                        if (line != null)
                        {
                            string[] attributes = line.Split(',');
                            _Timezones.Add(
                                    ushort.Parse(id),
                                    new Timezone(
                                        ushort.Parse(id),
                                        short.Parse(attributes[0].Trim()),
                                        DateOnly.ParseExact(attributes[1].Trim(), "ddMMyyyy", CultureInfo.InvariantCulture),
                                        DateOnly.ParseExact(attributes[2].Trim(), "ddMMyyyy", CultureInfo.InvariantCulture)
                                ));
                        }                        
                    }                  
                }
            }
        }

        public static Timezone GetById(ushort id)
        {
            return _Timezones[id];
        }
    }
}
