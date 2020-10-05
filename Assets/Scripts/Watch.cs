using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class Watch : Item
{
    //bools for storing information on the state of parts or existence of parts
    public bool casingBroken, mechanismBroken;
    public bool hasDecor;
    public bool[] componentBroken, hasMechComponent;

    //Has the watch been examined for a list
    public bool examined;

    /// <summary>
    /// Used for shuffling broken components
    /// 0 - 2 - Casing parts
    /// 3 - 5 - Mechanism parts
    /// 6 - Decoration
    /// </summary>
    public int[] componentOrder = new int[] { 0, 1, 2, 3, 4, 5, 6 };
    public int[] componentOrderNoDecor = new int[] { 0, 1, 2, 3, 4, 5 };

    //Amount of broken parts
    public int componentsToRepair;

    //ID of casing inside, probably obsolete
    public int casingID;

    //Casing component IDs
    public int[] componentID;
    //Mechanism component IDs
    public int[] mechComponentID;

    //Don't turn this to true!!!
    public bool testMode;

    // Start is called before the first frame update
    void Start()
    {
        RandomiseComponents(3, 7);
        ListComponents();

        //Multiplayer setup
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;

        //Getting components
        activator = GetComponentInParent<Activator>();
        itemSpriteRenderer = GetComponent<SpriteRenderer>();
        itemImage = itemSpriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        //Test mode randimisation
        if(Input.GetKeyDown("p") && testMode)
        {
            RandomiseComponents(3, 7);
            ListComponents();
        }

        //Sprite icons
        if (!knownState) stateSprite.sprite = GameManager.instance.unknownImage;
        else if (unfixable) stateSprite.sprite = GameManager.instance.unfixableImage;
        else if (broken) stateSprite.sprite = GameManager.instance.complexBrokenImage;
        else stateSprite.sprite = GameManager.instance.repairedImage;

        //Pickups - see Item scripts
        if (playerInRange)
        {
            //PickUp(0);

            //PickUp(1);
        }

        if (isSelected)
        {
            itemSpriteRenderer.sortingOrder = 2;
            itemSpriteRenderer.color = new Color(0.1f, 0.1f, 0.1f);
            if (!Physics2D.OverlapCircle(transform.position, 1, LayerMask.GetMask("Player")))
            {
                isSelected = false;
            }
        }
        else
        {
            itemSpriteRenderer.sortingOrder = 1;
            itemSpriteRenderer.color = Color.white;
        }
    }

    //Juts what it says on the tin
    public void ResetComponents()
    {
        componentBroken = new bool[7];
        hasMechComponent = new bool[3];
        casingBroken = false;
        mechanismBroken = false;
        broken = false;
    }

    //Randomises the watch
    //No more than 7 broken basic components
    public void RandomiseComponents(int minBroken, int maxBroken)
    {
        componentBroken = new bool[7];
        hasMechComponent = new bool[3];
        mechComponentID = new int[3];

        componentsToRepair = Random.Range(minBroken, maxBroken);
        Shuffle();

        for(int i = 0; i < componentsToRepair; i++)
        {
            if(hasDecor)
            {
                componentBroken[componentOrder[i]] = true;
            }
            else if(i != 6)
            {
                componentBroken[componentOrderNoDecor[i]] = true;
            }
        }

        if (componentBroken[0] || componentBroken[1] || componentBroken[2])
        {
            casingBroken = true;
        }
        else casingBroken = false;

        if (componentBroken[3] || componentBroken[4] || componentBroken[5])
        {
            mechanismBroken = true;
        }
        else mechanismBroken = false;
        
        for(int i = 3; i < 6; i++)
        {
            int rand = Random.Range(0, 2);
            if (!componentBroken[i] && rand == 0)
            {
                Debug.Log(rand);
                hasMechComponent[i - 3] = false;
            }
            else
            {
                hasMechComponent[i - 3] = true;
                mechComponentID[i - 3] = Random.Range(26, 43);
            }

        }
    }

    //Randomises the order of broken components
    public void Shuffle()
    {
        if(hasDecor)
        {
            int n = 7;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, 6);
                int value = componentOrder[k];
                componentOrder[k] = componentOrder[n];
                componentOrder[n] = value;
            }
        }
        else
        {
            int n = 6;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, 5);
                int value = componentOrderNoDecor[k];
                componentOrderNoDecor[k] = componentOrderNoDecor[n];
                componentOrderNoDecor[n] = value;
            }
        }
        
    }

    //Testing only - listing stuff in console
    private void ListComponents()
    {
        for (int i = 0; i < 7; i++)
        {
            Debug.Log("Component " + (i + 1) + " broken " + componentBroken[i]);
        }
        Debug.Log("Casing broken " + casingBroken);
        Debug.Log("Mech broken " + mechanismBroken);
        for(int i = 0; i < 3; i++)
        {
            Debug.Log("Mech component " + (i + 1) + " exists " + hasMechComponent[i]);
        }
    }
}
