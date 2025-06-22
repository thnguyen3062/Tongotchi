using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorToTop : MonoBehaviour
{
    private Camera mainCamera; // Assign the main camera in the inspector

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // If not assigned, use the main camera
        }
    }

    void Update()
    {
        // Get the top-center screen position in screen space
        Vector3 screenPosition = new Vector3(Screen.width / 2, Screen.height, mainCamera.nearClipPlane);

        // Convert screen position to world space
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // Set the GameObject's position to the world position
        transform.position = new Vector3(transform.position.x, worldPosition.y, transform.position.z);
    }
}
