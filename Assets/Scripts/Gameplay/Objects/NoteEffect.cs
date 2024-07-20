using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Helper;
using System.Diagnostics;
using Dremu.Gameplay.Tool;

namespace Dremu.Gameplay.Object {

    public class NoteEffect : MonoBehaviour, RecyclableObject {

        [SerializeField] AnimationCurve WidthCurve;
        [SerializeField] AnimationCurve AlphaCurve;
        [SerializeField] SpriteRenderer Renderer;

        private readonly Stopwatch stopwatch = new Stopwatch();

        public float Time { get { return stopwatch.ElapsedMilliseconds / 1000f; } }

        public void OnActive() {
            float time = stopwatch.ElapsedMilliseconds / 1000f;
            float width = WidthCurve.Evaluate(time);
            float alpha = AlphaCurve.Evaluate(time);

            transform.localScale = new Vector2(width, width);
            Renderer.color = UGUIHelper.SetAlpha(Renderer.color, alpha);
        }

        public void OnInitialize() { stopwatch.Restart(); }

        public void OnRecycle() {}
    }

}
