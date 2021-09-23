using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUnfader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance.transitionScreen.SetTrigger("FadeIn");
        Destroy(gameObject);
    }
}
