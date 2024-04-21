using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NsDataTest
{
    internal class TransitType
    {
        public TransitType(string code, string description) 
        {
            Code = code;
            Description = description;
        }
        public string Code { get; private set; }
        public string Description { get; private set; }

        private readonly static Dictionary<string, TransitType> _TransitTypes = new Dictionary<string, TransitType>();
        static TransitType()
        {
            using (FileStream fs = File.OpenRead("Dataset/trnsmode.dat"))
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
                        _TransitTypes.Add(
                            attributes[0].Trim(),
                            new TransitType(
                                attributes[0].Trim(),
                                attributes[1].Trim()
                        ));
                    }
                }
            }
        }

        public static TransitType GetByCode(string code)
        {
            return _TransitTypes[code];
        }
    }
}
