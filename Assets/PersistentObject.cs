using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is used to keep some objects (players, cameras, game manager) alive at all times
public class PersistentObject : MonoBehaviour
{
    public static PersistentObject instance;

    // Start is called before the first frame update
    void Start()
    {
        //Keeping the population of persistent objects in check
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
