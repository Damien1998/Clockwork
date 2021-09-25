using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedPanel : MonoBehaviour
{
    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void Appear()
    {
        if(myAnimator == null)
        {
            myAnimator = GetComponent<Animator>();
        }
        myAnimator.SetTrigger("Open");
    }

    public void Disappear()
    {
        if (myAnimator == null)
        {
            myAnimator = GetComponent<Animator>();
        }
        myAnimator.SetTrigger("Close");
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
