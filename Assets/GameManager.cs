using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class GameManager : MonoBehaviour
{
    public Activator[] items;
    public static GameManager instance;

    public Sprite repairedImage;
    public Sprite brokenImage;
    public Sprite unfixableImage;
    public Sprite unknownImage;
    public Sprite brokenImage2;

    public int minBrokenPieces = 1;
    public int maxBrokenPieces = 8;

    // Start is called before the first frame update
    void Start()
    {   
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
