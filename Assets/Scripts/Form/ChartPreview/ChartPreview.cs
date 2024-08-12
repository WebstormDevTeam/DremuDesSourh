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

public class ChartPreview : LabelWindowContent,IInputEventCallback,IRefresh,IRefreshCanvas
{
    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public void Refresh(int boxID, int lineID)
    {
        throw new NotImplementedException();
    }
}