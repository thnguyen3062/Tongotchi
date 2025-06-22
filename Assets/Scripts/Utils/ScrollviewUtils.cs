using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScrollviewUtils : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform sampleListItem;

    [SerializeField] private HorizontalLayoutGroup horizontalLG;

    private void Update()
    {
        int currentItem = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (sampleListItem.rect.width + horizontalLG.spacing));

        if (scrollRect.velocity.magnitude < 200)
            contentPanel.localPosition = new Vector3(currentItem * (sampleListItem.rect.width + horizontalLG.spacing), contentPanel.localPosition.y, contentPanel.localPosition.z);

    }
}
