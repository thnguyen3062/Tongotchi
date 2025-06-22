using System;
using TMPro;
using UnityEngine;

public class UINumericField : UITextField
{
    public void SetInterger(int value)
    {
        SetString(value.ToString());
    }

    public void SetFloat(float value, int roundAfterFloatPoint = 3)
    {
        SetString(Math.Round(value, roundAfterFloatPoint).ToString());
    }
}
