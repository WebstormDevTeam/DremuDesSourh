using Data.ChartData;
using Data.ChartEdit;
using Scenes.DontDestoryOnLoad;
using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Algorithm;
using GlobalData = Scenes.DontDestoryOnLoad.GlobalData;

using System.ComponentModel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Camera;

public class ChartPreview : LabelWindowContent,IInputEventCallback,IRefresh,IRefreshCanvas
{
    private void Start()
    {
        LoadChartPreview();
    }
    public void LoadChartPreview()
    {
        SceneManager.LoadSceneAsync("Scenes/Gameplay", LoadSceneMode.Additive);
    }
    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public void Refresh(int boxID, int lineID)
    {
        throw new NotImplementedException();
    }
}