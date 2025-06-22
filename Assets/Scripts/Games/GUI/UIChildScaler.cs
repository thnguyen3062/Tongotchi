using UnityEngine;
using UnityEngine.UI;

public class UIChildScaler : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private bool scaleVertical;
    [SerializeField] private bool scaleHorizontal;
    [SerializeField] private uint childPerLine = 2;
    [SerializeField] private float widthDecreasement = 0;
    [SerializeField] private RectTransform[] childArray;

    private GridLayoutGroup _gridLayout;

    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    public GridLayoutGroup GridLayout
    {
        get
        {
            if (_gridLayout == null)
            {
                _gridLayout = GetComponent<GridLayoutGroup>();
            }
            return _gridLayout;
        }
    }

    private void OnEnable()
    {
        if (GridLayout != null)
        {

        }
        else
        {
            ScaleChilds();
        }
    }

    private void OnDisable()
    {
        
    }

    private void ScaleChilds()
    {
        if (childArray.Length == 0) return;

        float widthValue = ((RectTransform.rect.width / childPerLine) - widthDecreasement);
        float heightValue = RectTransform.rect.height / Mathf.CeilToInt((float)childArray.Length / childPerLine);

        foreach (var child in childArray)
        {
            if (scaleHorizontal)
                child.SetRectWidth(widthValue);

            if (scaleVertical)
                child.SetRectHeight(heightValue);
        }
    }
}


public static class RectTransformExtentions
{
    public static void SetRectWidth(this RectTransform rect, float width)
    {
        Vector2 size = rect.sizeDelta;
        size.x = width;
        rect.sizeDelta = size;
    }

    public static void SetRectHeight(this RectTransform rect, float height)
    {
        Vector2 size = rect.sizeDelta;
        size.y = height;
        SetSize(rect, size);
    }

    public static void SetSize(this RectTransform rect, Vector2 size)
    {
        rect.sizeDelta = size;
    }
}