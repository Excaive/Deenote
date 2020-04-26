using UnityEngine;

namespace Deenote
{
    public static class Extensions
    {
        public static Color WithAlpha(this Color color, float alpha)
            => new Color(color.r, color.g, color.b, alpha);

        public static T[] WithValue<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
            return array;
        }
    }
}
