using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dremu.ChartEdit
{
    public class ContextMenu : MonoBehaviour
    {
        public List<GameObject> buttons;
        [SerializeField] GameObject buttonPrefab;
        public int Capacity
        {
            get
            {
                return buttons.Count;
            }
            set
            {
                for (int i = buttons.Count; i < value; i++)
                {
                    var btn = Instantiate(buttonPrefab, this.transform);
                    buttons.Add(btn);
                }
            }
        }

        public void SetButtonItems(List<ContextMenuItem> items)
        {
            Capacity = items.Count;
            foreach (var btn in buttons)
            {
                btn.GetComponent<Button>().onClick.RemoveAllListeners();
                btn.SetActive(false);
            }
            for (int i = 0; i < items.Count; i++)
            {
                var index = i;
                buttons[i].SetActive(true);
                buttons[i].GetComponent<Button>().onClick.AddListener(() => { items[index].callback(); this.gameObject.SetActive(!items[index].HideAfterClick); });
                buttons[i].GetComponentInChildren<Text>().text = items[i].Description;
            }
            var thisSize = this.GetComponent<RectTransform>().sizeDelta;
            var buttonSize = buttonPrefab.GetComponent<RectTransform>().sizeDelta;
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(thisSize.x, buttonSize.y * items.Count + 5);
        }
    }

    public class ContextMenuItem
    {
        public string Description;
        public UnityAction callback;
        public string HoverTip;
        public bool HideAfterClick;

        public ContextMenuItem(string str, UnityAction callback, string tip = "", bool hideAfterClick = true)
        {
            this.Description = str;
            this.callback = callback;
            this.HoverTip = tip;
            this.HideAfterClick = hideAfterClick;
        }
    }
}