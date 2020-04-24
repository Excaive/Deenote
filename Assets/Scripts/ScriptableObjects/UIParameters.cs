using UnityEngine;
using TMPro;

namespace Deenote
{
    [CreateAssetMenu(menuName = "Scriptable Objects/UI Parameters")]
    public sealed class UIParameters : ScriptableObject
    {
        // Localization and Fonts
        public TMP_FontAsset[] serifFonts;
        public TMP_FontAsset[] sansFonts;

        // Colors
        public Color transparent;
        public Color[] difficultyColors;
        public Color subBeatLineColor;
        public Color beatLineColor;
        public Color freeTempoLineColor;
        public Color tempoChangeLineColor;

        // Stage
        public Sprite[] difficultySprites;
        public Sprite pianoNoteSprite;
        public Sprite slideNoteSprite;
        public Sprite otherNoteSprite;
        public float[] orthogonalDistancesPerSecond;

        // Perspective view
        public Vector2 perspectiveRenderTextureSize;
        public Sprite[] noteDisappearingSprites;
        public float disappearingSpriteTimePerFrame;
        public float circleMaxScale;
        public float circleIncreaseTime;
        public float waveWidth;
        public float waveMaxScale;
        public float waveExpandTime;
        public float waveShrinkTime;
        public Color slideNoteWaveColor;
        public Color glowColor;
        public float glowWidth;
        public float glowMaxScale;
        public float glowExpandTime;
        public float glowShrinkTime;
        public float noteAnimationLength;
        public float pianoNoteScale;
        public float slideNoteScale;
        public float otherNoteScale;
        public float disappearingSpriteScale;
        public float[] perspectiveDistancesPerSecond;
        public float perspectiveMaxDistance;
        public float perspectiveOpaqueDistance;
        public float perspectiveHorizontalScale;
        public float comboNoNumberLength;
        public float comboNumberBlackOutTime;
        public float comboShadowMaxTime;
        public float comboShadowMinAlpha;
        public float comboShockWaveMaxTime;
        public float comboStrikeShowTime;
        public float comboCharmingExpandTime;
        public float comboCharmingShrinkTime;
        public float judgeLineEffectShrinkTime;
        public float lightEffectAngularFrequency;
        public float lightMaskMinScale;
        public float lightMaskMaxScale;

        // Other UI
        public float minDeltaAlpha;

        // Control
        public float epsilonTime;
        public float slowScrollSpeed;
        public float fastScrollSpeed;
    }
}
