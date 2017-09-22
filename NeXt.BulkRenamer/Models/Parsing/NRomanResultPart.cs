using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class NRomanResultPart : IResultPart
    {
        public NRomanResultPart(string format, int startValue = 1)
        {
            if (startValue < 0) throw new ArgumentOutOfRangeException(nameof(startValue));

            this.format = format;
            current = startValue - 1;
        }

        private readonly string format;

        private int current;
        
        public string Process(GroupCollection matches, FileInfo file)
        {
            return ToRoman(Interlocked.Increment(ref current), format);
        }
        
        private static readonly string[,] RomanNumerals = {
            {"", "I", "II", "III", "IV",   "V",    "VI",    "VII",    "VIII",    "IX"}, // ones
            {"", "X", "XX", "XXX", "XL",   "L",    "LX",    "LXX",    "LXXX",    "XC"}, // tens
            {"", "C", "CC", "CCC", "CD",   "D",    "DC",    "DCC",    "DCCC",    "CM"}, // hundreds
            {"", "M", "MM", "MMM", "MMMM", "MMMMM","MMMMMM","MMMMMMM","MMMMMMMM","MMMMMMMMM"} // thousands
        };
        
        private static string ToRoman(int number, string format)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (number < 0 || number > 9999) throw new ArgumentOutOfRangeException(nameof(number));
            
            var lower = format == "-";
            if (number == 0) return lower ? "n" : "N";
            
            var sb = new StringBuilder();

            for (var digits = (int) Math.Log10(number); digits >= 0; digits--)
            {
                sb.Append(RomanNumerals[digits, number / (int) Math.Pow(10, digits) % 10]);
            }

            return lower ? sb.ToString().ToLower() : sb.ToString();
        }
    }
}
