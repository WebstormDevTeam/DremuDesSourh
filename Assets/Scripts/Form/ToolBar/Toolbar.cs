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
using UnityEngine.UI;

public class Toolbar : LabelWindowContent
{
    [SerializeField] public Button openFileMenu;

    public void OnOpenFileMenuClicked()
    {
        Debug.Log("On open file menu clicked");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        openFileMenu.onClick.AddListener(OnOpenFileMenuClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
