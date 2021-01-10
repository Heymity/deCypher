using System;

namespace deCypher
{
    class Program
    {
        public static void Main(string[] args)
        {
            var encoded = CaesarCypher.Encode("Hi I`m Gabriel Pasquale", 3, null, false);

            Console.WriteLine(encoded);

            Console.WriteLine(CaesarCypher.Decode(encoded, 3));
            Console.WriteLine();

            var tmp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            CaesarCypher.MatchBruteForceDecode(encoded, "Gabriel").ForEach((string s) => Console.WriteLine(s));
            Console.WriteLine();
            foreach(var c in CaesarCypher.AllMatchBruteForceDecode(encoded, "Gabriel").results)
            {
                if (c.result == true) 
                { 
                    Console.ForegroundColor = ConsoleColor.Magenta; 
                    Console.Write(" > ");
                }
                else Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(c.value);
            }
            Console.ForegroundColor = tmp;

            encoded = Vigenere.Encode("Hi I`m Gabriel Pasquale", "test", null, false);

            Console.WriteLine(encoded);

            Console.WriteLine(Vigenere.Decode(encoded, "test", null, false));
        }
    }
}
