using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AddressableLoader : MonoBehaviour
{
    [SerializeField] private string m_AssetName;
    [SerializeField] private Image m_Image;

    private void Start()
    {
        if (string.IsNullOrEmpty(m_AssetName))
            return;

        m_Image.sprite = PlayerData.Instance.UISpriteDict[m_AssetName];
    }

#if UNITY_EDITOR
    private void Reset()
    {
        m_Image = GetComponent<Image>();
        if (!string.IsNullOrEmpty(m_Image.sprite.name))
            m_AssetName = m_Image.sprite.name;
    }
#endif
}
