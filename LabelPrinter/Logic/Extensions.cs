using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace LabelPrinter.Logic
{
    internal static class Extensions
    {
        public static Avalonia.Media.Color ToAvaloniaColor(this Color color)
        {
            var c = color.ToPixel<Rgba32>();

            return Avalonia.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static Color ToImageSharpColor(this Avalonia.Media.Color color)
        {
            return Color.FromRgba(color.R, color.G, color.B, color.A);
        }
    }
}
