using System;
using System.Collections.Generic;
using System.Text;

namespace deCypher
{
    public class Vigenere : ICypher<string>
    {
        public string text;
        public Alphabet alphabet = Alphabet.defaultAlphabet;
        public Alphabet keyAlphabet = Alphabet.defaultAlphabet;
        public bool ignoreCase = false;
        public bool ignoreKeyCase = false;
        public string key;

        public static string Encode(string text, string key, Alphabet alphabet = null, bool ignoreCase = true, bool ignoreKeyCase = true, Alphabet keyAlphabet = null)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            keyAlphabet ??= new Alphabet(alphabet.alphabet);

            if (ignoreCase)
            {
                alphabet.alphabet = alphabet.ListToLowerCase();
                text = text.ToLowerInvariant();
            }

            if(ignoreKeyCase)
                key.ToLowerInvariant();

            var newString = "";
            var keyIndex = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var tmp = alphabet.IndexOf(text[i]);
                var isUp = false;               
                if (tmp == -1)
                {
                    if (ignoreCase)
                    {
                        newString += text[i];
                        keyIndex++;
                        continue;
                    }
                    tmp = alphabet.ListToUpperCase().IndexOf(text[i]);
                    if (tmp == -1)
                    {
                        newString += text[i];
                        keyIndex++;
                        continue;
                    }
                    isUp = true;
                }
                var keyTmp = keyAlphabet.IndexOf(key[(i - keyIndex) % key.Length]);
                int upKeyTmp = -1;
                if (!ignoreKeyCase) upKeyTmp = keyAlphabet.ToUpperCase().IndexOf(key[(i - keyIndex) % key.Length]);
                tmp += keyTmp == -1 ? upKeyTmp == -1 ? 0 : upKeyTmp : keyTmp;
                if (tmp >= alphabet.Length) tmp -= alphabet.Length;
                if (isUp == true) newString += alphabet.ToUpperCase()[tmp];
                else newString += alphabet[tmp];
            }
            return newString;
        }

        public static string Decode(string text, string key, Alphabet alphabet = null, bool ignoreCase = true, bool ignoreKeyCase = true, Alphabet keyAlphabet = null)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            keyAlphabet ??= new Alphabet(alphabet.alphabet);
            var nkey = "";
            foreach(char c in key)
            {
                nkey += keyAlphabet[keyAlphabet.Length - keyAlphabet.IndexOf(c)];
            }
            return Encode(text, nkey, alphabet, ignoreCase, ignoreKeyCase, keyAlphabet);
        }

        public string Decode() => text = Decode(text, key, alphabet, ignoreCase, ignoreKeyCase, keyAlphabet);

        public string Encode() => text = Encode(text, key, alphabet, ignoreCase, ignoreKeyCase, keyAlphabet);
    }
}
