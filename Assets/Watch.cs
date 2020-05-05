using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : Item
{
    public bool casingBroken, mechanismBroken, hasDecor;
    public bool[] componentBroken, hasMechComponent;

    public int[] componentOrder = new int[] { 0, 1, 2, 3, 4, 5, 6 };
    public int[] componentOrderNoDecor = new int[] { 0, 1, 2, 3, 4, 5 };

    public int componentsBroken;
    public int casingID;
    public int[] componentID;
    public int[] mechComponentID;

    // Start is called before the first frame update
    void Start()
    {
        RandomiseComponents(3, 7);
        ListComponents();
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;
        itemImage = GetComponent<SpriteRenderer>().sprite;
        activator = GetComponentInParent<Activator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("p"))
        {
            RandomiseComponents(3, 7);
            ListComponents();
        }
        if (!knownState) stateSprite.sprite = unknownImage;
        else if (unfixable) stateSprite.sprite = unfixableImage;
        else if (broken) stateSprite.sprite = brokenImage;
        else stateSprite.sprite = repairedImage;

        if (playerCollided)
        {
            if (interactingPlayer[0] != null)
            {
                if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber) && !interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup)
                {
                    interactingPlayer[0].carriesItem = true;
                    interactingPlayer[0].freeToPickup = false;
                    interactingPlayer[0].itemSprite.sprite = itemImage;
                    interactingPlayer[0].itemStateSprite.sprite = stateSprite.sprite;
                    interactingPlayer[0].droppedItemActivator = activator;
                    gameObject.SetActive(false);
                }
            }
            if (interactingPlayer[1] != null)
            {
                if (Input.GetButton("Pickup" + interactingPlayer[1].playerNumber) && !interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup)
                {
                    interactingPlayer[1].carriesItem = true;
                    interactingPlayer[1].freeToPickup = false;
                    interactingPlayer[1].itemSprite.sprite = itemImage;
                    interactingPlayer[1].itemStateSprite.sprite = stateSprite.sprite;
                    interactingPlayer[1].droppedItemActivator = activator;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void ResetComponents()
    {
        componentBroken = new bool[7];
        hasMechComponent = new bool[3];
        casingBroken = false;
        mechanismBroken = false;
        broken = false;
    }

    //No more than 7 broken basic components
    public void RandomiseComponents(int minBroken, int maxBroken)
    {
        componentBroken = new bool[7];
        hasMechComponent = new bool[3];
        mechComponentID = new int[3];

        componentsBroken = Random.Range(minBroken, maxBroken);
        Shuffle();

        for(int i = 0; i < componentsBroken; i++)
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

    //Testing only
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
