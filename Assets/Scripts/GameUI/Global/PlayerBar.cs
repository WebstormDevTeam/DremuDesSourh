using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Global
{
    [ExecuteInEditMode] [DisallowMultipleComponent]
    public class PlayerBar : MonoBehaviour
    {
        [SerializeField] private Text playerName, ranking;

        private RectTransform _rect;
        private RectTransform RectTrans => _rect = _rect ? _rect : GetComponent<RectTransform>();

        private void OnEnable() => Match();

#if UNITY_EDITOR
        private void Update() => Match();
#endif

        private void Match()
        {
            RectTrans.anchoredPosition =
                new Vector2(845f, (float) Screen.height / Screen.width / (9f / 16f) * 540f - 90f);
        }

        //TODO：写好这个玩意
        public void Refresh()
        {
            
        }
    }
}
