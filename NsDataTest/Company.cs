using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NsDataTest
{
    public class Company
    {
        public Company(ushort id, string code, string name, ushort timezone) 
        {
            Id = id;
            Code = code;
            Name = name;
            Timezone = timezone;
        }


        public ushort Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public ushort Timezone { get; private set; }


        private readonly static Dictionary<ushort, Company> _Comanies = new Dictionary<ushort, Company>();

        static Company() 
        {
            using (FileStream fs = File.OpenRead("Dataset/company.dat"))
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
                        _Comanies.Add(
                            ushort.Parse(attributes[0]),
                            new Company(
                                ushort.Parse(attributes[0]),
                                attributes[1],
                                attributes[2],
                                ushort.Parse(attributes[3])
                        ));
                    }
                }
            }
        }

        public static Company GetById(ushort id)
        {
            return _Comanies[id];
        }
    }
}
