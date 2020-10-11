using System;
using System.Collections.Generic;

namespace deCypher
{
    class Program
    {
        public static void Main(string[] args)
        {
            var encoded = CaesarCypher.Encode("Hi I`m Gabriel Pasquale", 3);

            Console.WriteLine(encoded);

            Console.WriteLine(CaesarCypher.Decode(encoded, 3));
            Console.WriteLine();

            var tmp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            CaesarCypher.MatchBruteForceDecode(encoded, "Hi").ForEach((string s) => Console.WriteLine(s));
            Console.ForegroundColor = tmp;
        }
    }
}
