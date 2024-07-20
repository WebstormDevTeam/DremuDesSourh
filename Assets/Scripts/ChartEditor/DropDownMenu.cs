using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Dremu.ChartEdit
{

    public class DropDownMenu : MonoBehaviour
    {
        public void SetAttribute(Type targetEnum, UnityAction<object> reciever, int Default)
        {
            var dropDown = this.GetComponent<Dropdown>();
            dropDown.ClearOptions();
            dropDown.onValueChanged.RemoveAllListeners();
            var values = targetEnum.GetFields();
            List<Dropdown.OptionData> data = new();
            List<object> possibleValues = new();
            foreach (var field in values)
            {
                object[] attr = field.GetCustomAttributes(typeof(LabelTextAttribute), true);
                if (attr != null && attr.Length > 0)
                {
                    LabelTextAttribute descAttr = attr[0] as LabelTextAttribute;
                    data.Add(new Dropdown.OptionData(descAttr.Text));
                    possibleValues.Add(field.GetValue(null));
                }
            }
            dropDown.AddOptions(data);
            dropDown.value = Default;
            dropDown.onValueChanged.AddListener((value) => { this.gameObject.SetActive(false); reciever(possibleValues[value]); });
        }
    }
}