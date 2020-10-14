using System.Collections.Generic;

namespace deCypher
{
    public class CaesarCypher : ICypher<string>
    {
        public string text;
        public Alphabet alphabet = Alphabet.defaultAlphabet;
        public bool ignoreCase = false;
        public int rot = 3;

        public string Encode() => text = Encode(text, rot, alphabet, ignoreCase);
        public string Decode() => text = Decode(text, rot, alphabet, ignoreCase);
        public List<string> BruteForceDecode() => BruteForceDecode(text, alphabet, ignoreCase);
        public List<string> ShortBruteForceDecode(int sampleSize = 10) => ShortBruteForceDecode(text, sampleSize, alphabet, ignoreCase);
        public List<string> MatchBruteForceDecode(string match) => MatchBruteForceDecode(text, match, alphabet, ignoreCase);
        public (Dictionary<string, bool> results, List<int> matchIndexes) DictMatchBruteForceDecode(string match) => DictMatchBruteForceDecode(text, match, alphabet, ignoreCase);

        public CaesarCypher(string _text, Alphabet _alphabet, bool _ignoreCase, int _rot)
        {
            text = _text;
            alphabet = _alphabet;
            ignoreCase = _ignoreCase;
            rot = _rot;
        }

        public CaesarCypher(string _text)
        {
            text = _text;
        }

        public static string Encode(string text, int rot, Alphabet alphabet = null, bool ignoreCase = false)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            var newString = "";
            if (ignoreCase)
            {
                alphabet.alphabet = alphabet.ListToLowerCase();
                text = text.ToLowerInvariant();
            }
            for (var i = 0; i < text.Length; i++)
            {
                var tmp = alphabet.IndexOf(text[i]);
                var isUp = false;
                if (tmp == -1)
                {
                    if (ignoreCase)
                    {
                        newString += text[i];
                        continue;
                    }
                    tmp = alphabet.ListToUpperCase().IndexOf(text[i]);
                    if (tmp == -1)
                    {
                        newString += text[i];
                        continue;
                    }
                    isUp = true;
                }
                tmp += rot;
                if (tmp >= alphabet.Length) tmp -= alphabet.Length;
                if (isUp == true) newString += alphabet.ToUpperCase()[tmp];
                else newString += alphabet[tmp];
            }
            return newString;
        }

        public static string Decode(string text, int rot, Alphabet alphabet = null, bool ignoreCase = false)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            var nrot = alphabet.Length - rot;
            return Encode(text, nrot, alphabet, ignoreCase);
        }

        public static List<string> BruteForceDecode(string text, Alphabet alphabet = null, bool ignoreCase = false)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            var results = new List<string>();
            for (var i = 0; i <= alphabet.Length; i++)
            {
                results.Add(Encode(text, i, alphabet, ignoreCase));
            }
            return results;
        }

        public static List<string> ShortBruteForceDecode(string text, int sampleSize = 10, Alphabet alphabet = null, bool ignoreCase = false)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            if(text.Length > sampleSize) text = text.Remove(sampleSize);
            var results = new List<string>();
            for (var i = 0; i <= alphabet.Length; i++)
            {
                results.Add(Encode(text, i, alphabet, ignoreCase));
            }
            return results;
        }

        public static List<string> MatchBruteForceDecode(string text, string match, Alphabet alphabet = null, bool ignoreCase = false)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            var results = new List<string>();
            if (ignoreCase) match = match.ToLowerInvariant();
            for (var i = 0; i <= alphabet.Length; i++)
            {
                var decode = Encode(text, i, alphabet, ignoreCase);
                if (decode.Contains(match)) results.Add(decode);
            }
            return results;
        }

        public static (Dictionary<string, bool> results, List<int> matchIndexes) DictMatchBruteForceDecode(string text, string match, Alphabet alphabet = null, bool ignoreCase = false)
        {
            alphabet ??= Alphabet.defaultAlphabet;
            var results = new Dictionary<string, bool>();
            var matchIndexes = new List<int>();
            if (ignoreCase) match = match.ToLowerInvariant();
            for (var i = 0; i <= alphabet.Length - 1; i++)
            {
                var decode = Encode(text, i, alphabet, ignoreCase);
                var tmp = decode.Contains(match);
                results.Add(decode, tmp);
                if (tmp) matchIndexes.Add(i);
            }
            return (results, matchIndexes);
        }

    }
}
