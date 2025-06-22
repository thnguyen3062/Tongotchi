// Simple Scroll-Snap - https://assetstore.unity.com/packages/tools/gui/simple-scroll-snap-140884
// Copyright (c) Daniel Lochner

using UnityEngine;

namespace DanielLochner.Assets.SimpleScrollSnap
{
    public class TranslateZ : TransitionEffectBase<RectTransform>
    {
        private float yValue;
        private RectTransform rect;

        public override void OnTransition(RectTransform rectTransform, float distance)
        {
            if(rect == null)
            {
                rect = rectTransform;
                yValue = rect.localPosition.y;
            }
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, yValue + distance, 0);
        }
    }
}