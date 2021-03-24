#define DISPLAY
using deCypher.core.Steganography;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace deCypher
{
    class Program
    {
        public static void Main(string[] args)
        {
#if DISPLAY
            /**var encoded = CaesarCypher.Encode("Hi I`m Gabriel Pasquale", 3, null, false);

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

            Console.WriteLine(Vigenere.Decode(encoded, "test", null, false))*/

            const string inputPath = @"C:\Users\GABRIEL\Pictures\Workspace Images\MoonEclipse.png";
            const string outputPath = @"C:\Users\GABRIEL\Pictures\Workspace Images\MoonEclipse.out.png";

            Bitmap image = new Bitmap(inputPath);
            Steganography steg = new Steganography(image, outputPath);
            steg.LeastImportantBitsEncrypt("Lorem ipsum dolor sit amet, consectetur adipiscing elit.Quisque nunc tortor, malesuada a ligula non, eleifend hendrerit est.Curabitur fringilla, diam id aliquam rhoncus, diam massa vehicula erat, ut posuere diam augue et quam.Nullam consectetur leo massa, at suscipit quam pulvinar at.Fusce porta lectus faucibus eleifend elementum.Nam lacinia tristique tellus.Aliquam tempor lorem nec lectus volutpat, placerat dapibus augue suscipit.Nam pulvinar, quam eget rhoncus sagittis, diam risus vehicula risus, ut tristique lectus tellus at justo.Fusce eleifend mi et gravida pulvinar.Quisque a tortor non massa vehicula convallis et eget eros.Etiam tincidunt libero et convallis varius.Donec dictum pharetra velit, sed convallis ipsum congue vel.Morbi vitae interdum felis.Aliquam erat volutpat.Duis laoreet nunc sed sagittis mattis.Nulla dapibus ipsum sit amet eros lobortis, non ullamcorper lorem tristique.Proin egestas, ex vitae scelerisque sagittis, dui enim ullamcorper metus, nec dui. ");
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
            Console.ReadLine();
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
                    HandleFunctionCalling(typeof(CaesarCypher).GetMethod("Encode", BindingFlags.Static | BindingFlags.Public));
                    break;
                case "d":
                case "D":
                    HandleFunctionCalling(typeof(CaesarCypher).GetMethod("Decode", BindingFlags.Static | BindingFlags.Public));
                    break;
                case "b":
                case "B":
                    HandleFunctionCalling(typeof(CaesarCypher).GetMethod("BruteForceDecode", BindingFlags.Static | BindingFlags.Public));
                    break;
                case "s":
                case "S":
                    HandleFunctionCalling(typeof(CaesarCypher).GetMethod("ShortBruteForceDecode", BindingFlags.Static | BindingFlags.Public));
                    break;
                case "m":
                case "M":
                    HandleFunctionCalling(typeof(CaesarCypher).GetMethod("MatchBruteForceDecode", BindingFlags.Static | BindingFlags.Public));
                    break;
                case "a":
                case "A":
                    HandleFunctionCalling(typeof(CaesarCypher).GetMethod("AllMatchBruteForceDecode", BindingFlags.Static | BindingFlags.Public));
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
                    start:
                    //Console.Write(" (int): ");
                    var ans = Console.ReadLine();
                    if (ans == "" && op)
                    {
                        i++;
                        continue;
                    }
                    try
                    {
                        para[i] = int.Parse(ans);
                    } catch (Exception e)
                    {
                        Console.WriteLine($"Unable to parse to int. Try Again (error: {e.Message})");
                        goto start;
                    }
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

            object result = method.Invoke(null, para);
            Console.WriteLine();
            if (result.GetType() == typeof(string))
            {
                Console.WriteLine(result);
            }
            else if (result.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                var listResult = result as List<string>;
                foreach(string s in listResult)
                {
                    Console.WriteLine(s);
                }
            }
            else if(result is (List<(string, bool)> results, List<int> matchIndexes))
            {
                var res = ((List<(string value, bool result)> results, List<int> matchIndexes))result;
                foreach (var c in res.results)
                {
                    if (c.result == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(" > ");
                    }
                    else Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(c.value);
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("asdas");
            }
            else
            {
                Console.WriteLine(result);
            }
        }
    }
}
