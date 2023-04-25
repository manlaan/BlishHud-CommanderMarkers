using System;

namespace Manlaan.CommanderMarkers;

public static class MathUtils
{
    public const double DegToRad = Math.PI / 180;
    public const double RadToDeg = 1 / DegToRad;

    public const double AngleToRad = Math.PI;
    public const double RadToAngle = 1 / AngleToRad;

    public const double AngleToDeg = 180;
    public const double DegToAngle = 1 / AngleToDeg;

    public const float MetersToInches = 39.3700787f;
    public const float InchesToMeters = 1 / MetersToInches;

    public static double Squared(double x) => x * x;
    public static double Cubed(double x) => x * x * x;
    public static double Biquadrated(double x) => x * x * x * x;

    public static float Squared(float x) => x * x;

    public static double Clamp(double x, double min, double max)
        => Math.Min(Math.Max(x, min), max);

    public static double Clamp01(double x) => Clamp(x, 0, 1);

    public static double InverseLerp(double min, double max, double x, bool clamp = false)
        => ((clamp ? Clamp(x, min, max) : x) - min) / (max - min);

    public static double Lerp(double min, double max, double x, bool clamp = false)
        => min + (max - min) * (clamp ? Clamp01(x) : x);

    public static double Scale(double x, double sourceMin, double sourceMax, double targetMin, double targetMax,
        bool clamp = false)
        => Lerp(targetMin, targetMax, InverseLerp(sourceMin, sourceMax, x), clamp);
}