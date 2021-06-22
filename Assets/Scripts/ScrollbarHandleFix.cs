using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarHandleFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Scrollbar>().size = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
