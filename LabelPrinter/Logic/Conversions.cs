namespace LabelWhisper.Logic
{
    public static class Conversions
    {
        public static double MillimeterToPixel(double mm)
        {
            return 3.7795275591 * mm;
        }

        public static double PixelToMillimeter(double px)
        {
            return px * 0.2645833333;
        }
    }
}
