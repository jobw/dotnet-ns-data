using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NsDataTest
{
    internal static class Publication
    {
        static Publication()
        {
            using (FileStream fs = File.OpenRead("Dataset/delivery.dat"))
            using (StreamReader sr = new StreamReader(fs, Encoding.Latin1))
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                    string[] attributes = line.Split(',');

                    CompanyID = ushort.Parse(attributes[0].Replace("@", "").Trim());
                    ValidityStartDate = DateOnly.ParseExact(attributes[1].Trim(), "ddMMyyyy", CultureInfo.InvariantCulture);
                    ValidityEndDate = DateOnly.ParseExact(attributes[2].Trim(), "ddMMyyyy", CultureInfo.InvariantCulture);
                    Version = ushort.Parse(attributes[3].Trim());
                    Description = attributes[4].Trim();
                }
                else
                    throw new Exception("File was not able to be handled correctly");
                
            }
        }

        public static ushort CompanyID { get; private set; }
        public static DateOnly ValidityStartDate { get; private set; }
        public static DateOnly ValidityEndDate { get; private set; }
        public static  ushort Version { get; private set; }
        public static string Description { get; private set; }
    }
}
