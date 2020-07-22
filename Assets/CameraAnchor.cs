using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera camera = FindObjectOfType<Camera>();
        if (camera != null)
        {
            camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
