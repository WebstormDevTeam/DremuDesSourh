/*
 * Copyright (C) 2024 BlophyNovaEdit
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 注意，在没有修改的代码我们同样尊重MPL2.0的条款。只不过我们没有特别注明。
 * */
using System.Collections;
using Manager;
using Scenes.DontDestoryOnLoad;
using UnityEngine;
using UtilityCode.Singleton;
namespace Controller
{
    public class GameController : MonoBehaviourSingleton<GameController>
    {
        public bool isLoading;
        private IEnumerator Start()
        {
            isLoading = false;
            for (int i = 0; i < AssetManager.Instance.chartData.boxes.Count; i++)
            {
                Instantiate(AssetManager.Instance.boxController, AssetManager.Instance.box)
                    .SetSortSeed(i * ValueManager.Instance.noteRendererOrder)//这里的3是每一层分为三小层，第一层是方框渲染层，第二和三层是音符渲染层，有些音符占用两个渲染层，例如Hold，FullFlick
                    .Init(AssetManager.Instance.chartData.boxes[i]);
            }
            yield return new WaitForSeconds(3);//等8秒
            StateManager.Instance.IsStart = true;//设置状态IsStart为True

        }
        private void Update()
        {
            if( !(GlobalData.Instance.chartData.globalData.musicLength - ProgressManager.Instance.CurrentTime <= .1f) || isLoading )
                return;
            isLoading = true;

        }
    }
}
