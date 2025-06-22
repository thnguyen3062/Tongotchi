using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera m_MainCamera;
    public Transform player;  // The player object
    public RectTransform bottomUI;  // The UI element

    private float referenceResolutionHeight = 1280f;
    private float referenceDistance = 0.1f;

    public void Start()
    {
        AdjustCamera();
    }

    private void AdjustCamera()
    {
        m_MainCamera.orthographicSize = 1.92f * Screen.height / Screen.width * 0.5f;

        if (Screen.height < referenceResolutionHeight)
        {
            // Get the world position of the bottom UI element
            Vector3 bottomUIWorldPosition = GetBottomUIPositionInWorldSpace();
            float distance = player.position.y - bottomUIWorldPosition.y;
            float ration = Screen.height / referenceResolutionHeight;
            float cameraOffsetY = (referenceDistance - distance * ration);
            m_MainCamera.transform.position = new Vector3(m_MainCamera.transform.position.x, player.position.y - cameraOffsetY, -10);
        }
    }

    Vector3 GetBottomUIPositionInWorldSpace()
    {
        Vector3 uiScreenPosition = RectTransformUtility.WorldToScreenPoint(m_MainCamera, bottomUI.position);
        Vector3 bottomUIWorldPosition = m_MainCamera.ScreenToWorldPoint(uiScreenPosition);
        return bottomUIWorldPosition;
    }
}