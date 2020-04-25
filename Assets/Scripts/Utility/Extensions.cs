using UnityEngine;

namespace Deenote
{
    public static class Extensions
    {
        public static Color WithAlpha(this Color color, float alpha)
            => new Color(color.r, color.g, color.b, alpha);
    }
}
