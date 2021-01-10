//#define TESTING
using System;
using System.Collections.Generic;
using System.Reflection;

namespace deCypher
{
    class Program
    {
        public static void Main(string[] args)
        {
#if TESTING
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
#else
            Console.WriteLine("Please select the function to be called: (Type the letter in []. Case insensitive)");
            Console.WriteLine("[C]aesar Cypher\n[V]igenere Cypher");
            var ans = Console.ReadLine();
            switch (ans)
            {
                case "v":
                case "V":
                    HandleVigenere();
                    break;
                case "c":
                case "C":
                    HandleCaesar();
                    break;
            }
#endif
        }

        public static void HandleVigenere()
        {
            Console.WriteLine("Please select the funticon to be called:");
            Console.WriteLine("[E]ncode(text, key)\n[D]ecode(text, key)");
            var ans = Console.ReadLine();
            switch (ans)
            {
                case "e":
                case "E":                 
                    HandleFunctionCalling(typeof(Vigenere).GetMethod("Encode", BindingFlags.Static | BindingFlags.Public));
                    break;
                case "d":
                case "D":
                    HandleFunctionCalling(typeof(Vigenere).GetMethod("Decode", BindingFlags.Static | BindingFlags.Public));
                    break;
            }
        }
        public static void HandleCaesar()
        {
            Console.WriteLine("Please select the funticon to be called:");
            Console.WriteLine("[E]ncode(text, rotation)\n[D]ecode(text, rotation)\n[B]ruteForceDecode(text)\n[S]hortBruteForceDecode(text, [sampleSize = 10])\n[M]atchBruteForceDecode(text, match)\n[A]llMathBruteForceDecode(text, match)");
            var ans = Console.ReadLine();
            switch (ans)
            {
                case "e":
                case "E":
                    break;
                case "d":
                case "D":
                    break;
                case "b":
                case "B":
                    break;
                case "s":
                case "S":
                    break;
                case "m":
                case "M":
                    break;
                case "a":
                case "A":
                    break;
            }
        }

        public static void HandleFunctionCalling(MethodInfo method)
        {
            if (method == null) return;

            ParameterInfo[] parames = method.GetParameters();
            Console.WriteLine("Input Optional Values? Y/N");
            var yn = Console.ReadLine();
            bool ov = false;

            if (yn == "y" || yn == "Y") ov = true;

            object[] para = new object[parames.Length];
            int i = 0;
            foreach(ParameterInfo p in parames)
            {
                var op = p.IsOptional;
                if (op && !ov) break;
                Console.Write($"{p.Name} ({p.ParameterType}{(op ? ", default: " + (p.DefaultValue == null ? "null" : p.DefaultValue) : "")}): ");
                if (p.ParameterType == typeof(int))
                {
                    //Console.Write(" (int): ");
                    var ans = Console.ReadLine();
                    if (ans == "" && op)
                    {
                        i++;
                        continue;
                    }
                    para[i] = int.Parse(ans);
                }
                else if (p.ParameterType == typeof(string))
                {
                    var ans = Console.ReadLine();
                    if (ans == "" && op)
                    {
                        i++;
                        continue;
                    }
                    para[i] = ans;
                }
                else if (p.ParameterType == typeof(bool))
                {
                    start:
                    var ans = Console.ReadLine();
                    if (ans == "" && op)
                    {
                        i++;
                        continue;
                    }
                    switch (ans)
                    {
                        case "0":
                        case "f":
                        case "F":
                        case "n":
                        case "N":
                        case "false":
                        case "False":
                        case "FALSE":
                            para[i] = false;
                            break;
                        case "1":
                        case "t":
                        case "T":
                        case "y":
                        case "Y":
                        case "true":
                        case "True":
                        case "TRUE":
                            para[i] = true;
                            break;
                        default:
                            Console.WriteLine($"{ans} is not supported. Try 0; f; F; n; N; false; False; FALSE; 1; t; T; y; Y; true; True; TRUE;");
                            goto start;
                    }
                }
                else if (p.ParameterType == typeof(Alphabet))
                {
                    var ans = Console.ReadLine();
                    if (ans == "" && op)
                    {
                        i++;
                        continue;
                    }
                    para[i] = new Alphabet(ans);
                }
                else Console.WriteLine();
                i++;
            }

            Console.WriteLine(method.Invoke(null, para));
        }
    }
}
