using System;
using UnityEngine;

namespace Extensions
{
    public static class CustomMathExtensions
    {
        public enum ExcludeAxis
        {
            X,
            Y,
            Z,
            XY,
            XZ,
            ZY, None
        }


        /// <summary>
        /// Clamps the current value between a minimum and maximum value using the Mathf.Clamp function.  Note that if minValue and/or maxValue are left unassigned, it will clamp between the minimum value and maximum value of float (float.MinValue/float.MaxValue).
        /// </summary>
        /// <param name="value">The current value to be clamped</param>
        /// <param name="minValue">The smallest number the current number can be.</param>
        /// <param name="maxValue">The highest number the current number can be.</param>
        /// <returns></returns>
        public static float ClampedTo(this float value, float minValue = float.MinValue,
            float maxValue = float.MaxValue)
        {
            return Mathf.Clamp(value, minValue, maxValue);
        }

        public static Vector3 ClampMagnitude(Vector3 value, float maxLength, ExcludeAxis axis)
        {
            Vector3 clampedResults = Vector3.ClampMagnitude(value, maxLength);
            switch (axis)
            {
                default:
                    return clampedResults;
                case ExcludeAxis.X:
                    return new Vector3(value.x, clampedResults.y, clampedResults.z);
                case ExcludeAxis.Y:
                    return new Vector3(clampedResults.x, value.y, clampedResults.z);
                case ExcludeAxis.Z:
                    return new Vector3(clampedResults.x, clampedResults.y, value.z);
                case ExcludeAxis.XY:
                    return new Vector3(value.x, value.y, clampedResults.z);
                case ExcludeAxis.XZ:
                    return new Vector3(value.x, clampedResults.y, value.z);
                case ExcludeAxis.ZY:
                    return new Vector3(clampedResults.x, value.y, value.z);
            }
        }
    }
}