/*
 * Copyright (C) 2024 BlophyNovaEdit
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 注意，在没有修改的代码我们同样尊重MPL2.0的条款。只不过我们没有特别注明。
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Camera;
public class LabelWindowContent : MonoBehaviour,IInputEventCallback
{
    [SerializeField]public string labelWindowName;
    [SerializeField]public LabelWindow labelWindow;
    [SerializeField]public LabelWindowContentType labelWindowContentType;
    [SerializeField]public int minX=100;
    [SerializeField]public int minY=100;
    public Vector2 MousePositionInThisRectTransform => (Vector2)transform.InverseTransformPoint(main.ScreenToWorldPoint(Mouse.current.position.value)) + labelWindow.labelWindowRect.sizeDelta / 2;
    public virtual void WindowSizeChanged(){}
    public virtual void Started(InputAction.CallbackContext callbackContext){ }
    public virtual void Performed(InputAction.CallbackContext callbackContext){}
    public virtual void Canceled(InputAction.CallbackContext callbackContext){}
}
