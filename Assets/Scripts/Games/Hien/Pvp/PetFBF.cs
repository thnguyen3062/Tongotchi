using UnityEngine;
using UnityEngine.UI;

public class PetFBF : MonoBehaviour
{
    [SerializeField] protected int idleFramePerSecond = 4;
    [SerializeField] protected Image m_PetImg;

    protected Sprite[] petIdleSprites = new Sprite[0];

    protected float framePerSecond;
    protected float timer;
    protected int frameIndex;

    public void SetSprites(string idlePath)
    {
        petIdleSprites = Resources.LoadAll<Sprite>(idlePath);
        framePerSecond = idleFramePerSecond;
    }

    public virtual void Refresh()
    {
        petIdleSprites = new Sprite[0];
        timer = 0f;
        frameIndex = 0;
    }

    protected virtual void Update()
    {
        if (petIdleSprites.Length == 0 || m_PetImg == null)
            return;

        timer += Time.deltaTime;
        if (timer >= 1f / idleFramePerSecond)
        {
            timer -= 1f / idleFramePerSecond;
            frameIndex = (frameIndex + 1) % petIdleSprites.Length;
            m_PetImg.sprite = petIdleSprites[frameIndex];
        }
    }
}
