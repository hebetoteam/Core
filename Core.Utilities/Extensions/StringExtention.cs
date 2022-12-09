using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Utilities.Extensions
{
    public static class StringExtention
    {
        private static Random random = new Random();
        public static bool EndWith(this string value, params string[] compareString)
        {
            foreach (var item in compareString)
            {
                var result = value.EndsWith(item);

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Equals(this string value, params string[] compareString)
        {
            foreach (var item in compareString)
            {
                var result = value.Equals(item);

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMissing(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string MaskString(this string value)
        {
            var left = value.Substring(value.Length - 8);
            string right = value.Substring(value.Length - 6, 6);

            return string.Format("{0}.......{1}", left, right);
        }

        public static string ToddMMyyyyHHmmss(this DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }



        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
