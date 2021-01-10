using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace deCypher
{
    public class Alphabet
    {
        public static readonly Alphabet defaultAlphabet = new Alphabet("abcdefghijklmnopqrstuvwxyz");
        public static readonly Alphabet complexAlphabet = new Alphabet("a1b2cç3d4e5f6g7h8i9j0k-l=m_nñ+o`p~q[r]s{t}u;v\"w;x'y/z?<.>\\|!)@(#*$&%^´");
        public static readonly Alphabet ASCIIAlphabet = new Alphabet(" !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");


        public List<char> alphabet;
        public string StringValue
        {
            get
            {
                string s = "";
                foreach (char c in alphabet)
                {
                    s += c;
                }
                return s;
            }
            set => alphabet = new List<char>(value.ToCharArray().Distinct());
        }
        public int Length => alphabet.Count;

        public char this[int i]
        {
            get => alphabet[i];
            set => alphabet[i] = value;
        }

        public Alphabet(string content)
        {
            alphabet = new List<char>(content.ToCharArray().Distinct());
        }

        public Alphabet(List<char> content)
        {
            alphabet = new List<char>(content);
        }

        public string ToLowerCase() => StringValue.ToLowerInvariant();
        public List<char> ListToLowerCase() => alphabet.ConvertAll((char c) => c.ToString().ToLowerInvariant().ToCharArray()[0]);
        public string ToUpperCase() => StringValue.ToUpperInvariant();
        public List<char> ListToUpperCase() => alphabet.ConvertAll((char c) => c.ToString().ToUpperInvariant().ToCharArray()[0]);
        public int IndexOf(char c) => alphabet.IndexOf(c);
    }
}
