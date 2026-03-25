using System;
using System.Drawing;
namespace OmniFlex.Models.Services
{
    public static class EnrollmentHelper
    {

        public static string GetRandomDarkHex()
        {
            Color color;
            double luminance;

            do
            {
                // Generate random RGB components
                int r = Random.Shared.Next(256);
                int g = Random.Shared.Next(256);
                int b = Random.Shared.Next(256);

                color = Color.FromArgb(r, g, b);

                // Calculate Relative Luminance based on WCAG standards
                double GetL(double c) => (c / 255.0 <= 0.03928)
                    ? (c / 255.0 / 12.92)
                    : Math.Pow((c / 255.0 + 0.055) / 1.055, 2.4);

                luminance = 0.2126 * GetL(color.R) + 0.7152 * GetL(color.G) + 0.0722 * GetL(color.B);

                // 0.179 is the threshold where white text remains readable (approx 4.5:1 ratio)
            } while (luminance > 0.179);

            // Short-one liner:
            // string hex = $"#{Random.Shared.Next(0x1000000) & 0x7F7F7F:X6}";

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}