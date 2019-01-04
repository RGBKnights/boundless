using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmic.Generation
{
    public static class MyExtensions
    {
        public static string GenerateName(this Random rnd, int length = 7)
        {
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };

            var Name = new StringBuilder();
            Name.Append(consonants[rnd.Next(consonants.Length)].ToUpper());
            Name.Append(vowels[rnd.Next(vowels.Length)]);

            int b = 2; // b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < length)
            {
                Name.Append(consonants[rnd.Next(consonants.Length)]);
                b++;
                Name.Append(vowels[rnd.Next(vowels.Length)]);
                b++;
            }

            return Name.ToString();
        }

        public static Range RandomRange(this Random rnd, int min, int max)
        {
            var low = rnd.Next(min, max);
            var high = rnd.Next(low, max);
            return new Range(low, high);
        }
    }

    public static class MathEx
    {
        public static double NthRoot(double input, int N)
        {
            return Math.Pow(input, 1.0 / N);
        }
    }
}
