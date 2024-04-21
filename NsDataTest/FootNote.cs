using System.Text;

namespace NsDataTest
{
    internal class FootNote
    {
        public FootNote(ushort id, Dictionary<DateOnly, bool> validity) 
        {
            Id = id;
            Validity = validity;
        }

        public ushort Id { get; private set; }
        public Dictionary<DateOnly, bool> Validity { get; private set; }


        private readonly static Dictionary<ushort, FootNote> _Footnotes 
            = new Dictionary<ushort, FootNote>();
        static FootNote()
        {
            using (FileStream fs = File.OpenRead("Dataset/footnote.dat"))
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
                            char[] validity_string = line.ToCharArray();

                            Dictionary<DateOnly, bool> temp_validity 
                                = new Dictionary<DateOnly, bool>();
                            for (int i = 0; i < validity_string.Length; i++) 
                            {
                                temp_validity.Add(
                                    Publication.ValidityStartDate.AddDays(i), 
                                    validity_string[i] == '1');
                            }

                            _Footnotes.Add(
                                    ushort.Parse(id),
                                    new FootNote(
                                        ushort.Parse(id),
                                        temp_validity
                                ));
                        }
                    }
                }
            }
        }

        public static FootNote GetById(ushort id)
        {
            return _Footnotes[id];
        }
    }
}
