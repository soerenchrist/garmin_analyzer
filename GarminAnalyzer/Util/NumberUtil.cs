namespace GarminAnalyzer.Util
{
    public static class NumberUtil
    {
        public static double Normalize(double value, double min, double max)
        {
            return (value - min) / (max - min);
        }
    }
}