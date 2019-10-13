using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DBNetEngine
{
    public static class MathSupport
    {
        public const float MaxValue = 32000;

        public static Vector2 MaxVector { get; } = new Vector2(MaxValue, MaxValue);
        public static Vector2 MinVector { get; } = new Vector2(-MaxValue, -MaxValue);

        public static float AngleDifference(float angle1, float angle2)
        {
            return AngleNormalise(angle1 - angle2);
        }

        public static float AngleNormalise(float angle)
        {
            while (angle < 0)
                angle += (float)(2 * Math.PI);
            while (angle > (float)(2 * Math.PI))
                angle -= (float)(2 * Math.PI);

            return angle;
        }

        public static int AngleToInteger(float angle)
        {
            return (int)Math.Round(angle * 200);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CrossProduct(this Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.Y
                  - value1.Y * value2.X;
        }

        public static float IntegerToAngle(int angle)
        {
            return (float)angle / 200;
        }
    }
}