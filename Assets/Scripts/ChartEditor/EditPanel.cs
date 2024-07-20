using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Dremu.ChartEdit
{

    public class EditPanel : MonoBehaviour
    {
        [SerializeField] Text ShowText;
        [SerializeField] InputField Label;
        [SerializeField] Button SubmitButton;
        [SerializeField] Button CancelButton;
        bool _waiting = false;

        public void SetAttribute(string text, string placeHolder, UnityAction<string> submitCallback, string defaultText = "")
        {
            this.Reset();
            this._waiting = true;
            this.ShowText.text = text;
            this.Label.text = defaultText;
            EventSystem.current.SetSelectedGameObject(Label.gameObject);
            this.Label.placeholder.GetComponent<Text>().text = placeHolder;
            this.Label.onSubmit.AddListener((value) => { submitCallback(value); this.Reset(); });
            this.SubmitButton.onClick.AddListener(() => { submitCallback(this.Label.text); this.Reset(); });
            this.CancelButton.onClick.AddListener(Reset);
        }

        public void Reset()
        {
            this.Label.onSubmit.RemoveAllListeners();
            this.SubmitButton.onClick.RemoveAllListeners();
            this.CancelButton.onClick.RemoveAllListeners();
            this.Label.text = "";
            if (this._waiting)
            {
                this._waiting = false;
                this.gameObject.SetActive(false);
            }
        }
    }
}