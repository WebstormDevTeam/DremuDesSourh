using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Dremu.ChartEdit.Envelope
{
    [RequireComponent(typeof(UILineRenderer))]
    public class UILine: MonoBehaviour
    {
        public float size = 5;
        public List<Vector2> points = new List<Vector2>();
        public void Update()
        {
            var renderer = this.GetComponent<UILineRenderer>();
            
            renderer.Points = points.ToArray();
            renderer.SetAllDirty();
        }
    }
}

