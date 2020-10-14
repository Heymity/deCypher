using System;
using System.Collections.Generic;
using System.Text;

namespace deCypher
{
    public class Vigenere : ICypher<string>
    {
        public static string Encode(string text, int rot, Alphabet alphabet = null, bool ignoreCase = false)
        {
            return "";
        }

        public string Decode()
        {
            throw new NotImplementedException();
        }

        public string Encode()
        {
            throw new NotImplementedException();
        }
    }
}
