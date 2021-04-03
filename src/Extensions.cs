using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogParser
{
    public static class Extensions
    {
        public static Dictionary<string, string> SplitToDictionary(this string text, string separator, int skip = 0)
        {
            var pairs = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (skip > 0 && pairs.Length > skip)
                pairs = pairs.Skip(skip).ToArray();
            var dict = new Dictionary<string, string>();
            var key = "";
            for (var i = 0; i < pairs.Length; i++)
            {
                if (i % 2 == 0)
                    key = pairs[i];
                else
                    dict[key] = pairs[i];
            }
            return dict;
        }

        public static string ToTimeSpan(this TimeSpan ts)
        {
            if (ts.TotalDays > 1)
                return ts.ToString("d'd 'h'h'");
            else if (ts.TotalHours > 1)
                return ts.ToString("h'h 'm'm'");
            else
                return ts.ToString("m'm'");
        }

        public static string PureName(this string name)
        {
            return Regex.Replace(name, @"\^\d", "");
        }


        static readonly Dictionary<char, string> Colors = new Dictionary<char, string>
        {
            { '1', "#F00" },
            { '2', "#FC0" },
            { '3', "#0F0" },
            { '4', "#00F" },
            { '5', "#0FF" },
            { '6', "#F0F" },
            { '7', "#FFF" },
        };

        public static string FormatToHtml(this string name)
        {
            if (name == null || !name.Contains('^'))
                return name;

            var ar = name.Split('^', StringSplitOptions.None);
            var sb = new StringBuilder();

            if (ar[0].Length > 0)
                sb.AppendFormat("<span style=\"color:black\">{0}</span>", ar[0]);

            for (var i = 1; i < ar.Length; i++)
            {
                var c = ar[i].First();
                if (Colors.ContainsKey(c))
                    sb.AppendFormat("<span style=\"color:{0}\">{1}</span>", Colors[c], ar[i].Substring(1));
                else
                    sb.AppendFormat("<span style=\"color:black\">{0}</span>", ar[i].Substring(1));
            }

            return sb.ToString();
        }

        public static IEnumerable<int> ToInc(this IEnumerable<int> e)
        {
            var last = 0;
            foreach (var ei in e)
            {
                last = ei + last;
                yield return last;
            }
        }
    }
}
