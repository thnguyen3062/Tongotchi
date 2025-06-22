using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownOverride : MonoBehaviour
{
    private Canvas canvas;
    private void OnEnable()
    {
        if (GetComponent<Canvas>() == null)
            return;

        if (canvas == null)
            canvas = GetComponent<Canvas>();

        canvas.sortingOrder = 1000;
    }
}
