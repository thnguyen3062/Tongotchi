using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnEndAnim()
    {
        this.gameObject.SetActive(false);
    }    
}
