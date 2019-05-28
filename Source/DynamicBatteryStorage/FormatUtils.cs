using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{

    public static class FormatUtils
    {
        public const double KERBIN_DAY_LENGTH = 6d;
        public const double KERBIN_YEAR_LENGTH = 426d;
        public const double EARTH_DAY_LENGTH = 24d;
        public const double EARTH_YEAR_LENGTH = 365d;

        /// <summary>
        /// Converts a number into a SI prefix/suffixed version
        /// </summary>
        /// <returns>String</returns>
        /// <param name="d">D.</param>
        /// <param name="format">Format.</param>
        public static string ToSI(double d, string format = null)
        {
            if (d == 0.0)
                return d.ToString(format);

            char[] incPrefixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
            char[] decPrefixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

            int degree = Mathf.Clamp((int)Math.Floor(Math.Log10(Math.Abs(d)) / 3), -8, 8);
            if (degree == 0)
                return d.ToString(format) + " ";

            double scaled = d * Math.Pow(1000, -degree);

            char? prefix = null;

            switch (Math.Sign(degree))
            {
                case 1: prefix = incPrefixes[degree - 1]; break;
                case -1: prefix = decPrefixes[-degree - 1]; break;
            }

            return scaled.ToString(format) + " " + prefix;
        }

        /// <summary>
        /// Converts a seconds value into a kerbin/real formatted time string
        /// </summary>
        /// <returns>The time string.</returns>
        /// <param name="seconds">Seconds.</param>
        public static string FormatTimeString(double seconds)
        {
            double dayLength;
            double yearLength;
            double rem;

            if (GameSettings.KERBIN_TIME)
            {
                dayLength = KERBIN_DAY_LENGTH;
                yearLength = KERBIN_YEAR_LENGTH;
            }
            else
            {
                dayLength = EARTH_DAY_LENGTH;
                yearLength = EARTH_YEAR_LENGTH;
            }

            int years = (int)(seconds / (3600.0d * dayLength * yearLength));
            rem = seconds % (3600.0d * dayLength * yearLength);
            int days = (int)(rem / (3600.0d * dayLength));
            rem = rem % (3600.0d * dayLength);
            int hours = (int)(rem / (3600.0d));
            rem = rem % (3600.0d);
            int minutes = (int)(rem / (60.0d));
            rem = rem % (60.0d);
            int secs = (int)rem;

            string result = "";

            // draw years + days
            if (years > 0)
            {
                result += years.ToString() + "y ";
                result += days.ToString() + "d ";
                result += hours.ToString() + "h ";
                result += minutes.ToString() + "m";
            }
            else if (days > 0)
            {
                result += days.ToString() + "d ";
                result += hours.ToString() + "h ";
                result += minutes.ToString() + "m ";
                result += secs.ToString() + "s";
            }
            else if (hours > 0)
            {
                result += hours.ToString() + "h ";
                result += minutes.ToString() + "m ";
                result += secs.ToString() + "s";
            }
            else if (minutes > 0)
            {
                result += minutes.ToString() + "m ";
                result += secs.ToString() + "s";
            }
            else if (seconds > 0)
            {
                result += secs.ToString() + "s";
            }
            else
            {
                result = "< 1s";
            }


            return result;
        }
    }
}
