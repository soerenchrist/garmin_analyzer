using System.Windows.Media;

namespace GarminAnalyzer.Util
{
    public static class ColorHelper
    {

        public static SolidColorBrush NumberToColor(double value)
        {
            if (value > 100) value = 100;
            
            var ratio = value / 100;

            var saturation = ratio * 255;
            
            return new SolidColorBrush(Color.FromRgb((byte)saturation, (byte)(255 - saturation), (byte)(255- saturation)));
        }
        
    }
}