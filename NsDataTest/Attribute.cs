using System.Text;

namespace NsDataTest
{
    internal class Attribute
    {
        public Attribute(string code, ushort handlingCode, string description) 
        {
            Code = code;
            HandlingCode = handlingCode;
            Description = description;
        }

        public string Code { get; private set; }
        public ushort HandlingCode { get; private set; }
        public string Description { get; private set; }

        private readonly static Dictionary<string, Attribute> _Attributes 
            = new Dictionary<string, Attribute>();
        static Attribute()
        {
            using (FileStream fs = File.OpenRead("Dataset/trnsattr.dat"))
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
                        _Attributes.Add(
                            attributes[0].Trim(),
                            new Attribute(
                                attributes[0].Trim(),
                                ushort.Parse(attributes[1].Trim()),
                                attributes[2].Trim()
                        ));
                    }
                }
            }
        }

        public static Attribute GetByCode(string code)
        {
            return _Attributes[code];
        }
    }
}
