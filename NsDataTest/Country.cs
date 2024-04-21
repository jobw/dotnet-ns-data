using System.Text;

namespace NsDataTest
{
    internal class Country
    {
        public Country(string code, bool domestic, string name) 
        {
            Code = code;
            IsDomestic = domestic;
            Name = name;
        }

        public string Code { get; private set; }
        public bool IsDomestic { get; private set; }
        public string Name { get; private set; }

        private readonly static Dictionary<string, Country> _Countries 
            = new Dictionary<string, Country>();
        static Country()
        {
            using (FileStream fs = File.OpenRead("Dataset/country.dat"))
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
                        _Countries.Add(
                            attributes[0].Trim(),
                            new Country(
                                attributes[0].Trim(),
                                attributes[1].Trim() == "1",
                                attributes[2].Trim()
                        ));
                    }
                }
            }
        }

        public static Country GetByCode(string code)
        {
            return _Countries[code];
        }
    }
}
