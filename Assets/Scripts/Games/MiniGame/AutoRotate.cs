using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotationSpeed = 10f;
    private MinigameData minigameData;
    private void Awake()
    {
        minigameData = MiniGameDataSO.Instance.baseMiniGameData;
        this.rotationSpeed = minigameData.trapRotationSpeed;
    }
    void Update()
    {

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
