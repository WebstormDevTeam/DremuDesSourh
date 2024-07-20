using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameUI.Global
{
    public class UIOutline : Shadow
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            if (this.IsActive())
            {
                List<UIVertex> list = new List<UIVertex>();
                vh.GetUIVertexStream(list);
                int num = list.Count;
                if (list.Capacity < num)
                {
                    list.Capacity = num;
                }

                var pos = base.effectDistance;
                int start = 0;
                int count = list.Count;
                ApplyShadow(list, (Color32) base.effectColor, start, list.Count, 0, pos.y);
                ApplyShadow(list, (Color32) base.effectColor, start, list.Count, 0, -pos.y);
                ApplyShadow(list, (Color32) base.effectColor, start, list.Count, pos.x, 0);
                ApplyShadow(list, (Color32) base.effectColor, start, list.Count, -pos.x, 0);
                vh.Clear();
                vh.AddUIVertexTriangleStream(list);
            }
        }
    }
}
