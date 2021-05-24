using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableOrderSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private int baseSortingOrder;

    [SerializeField] private bool multipleSprites;

    [SerializeField] private Transform frontPoint, backPoint;

    [SerializeField] private SpriteRenderer[] multipleVariableSprites;
    private List<int> baseSortingOrders;

    // Start is called before the first frame update
    void Start()
    {
        if(!multipleSprites)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseSortingOrder = spriteRenderer.sortingOrder;
        }
        else
        {
            baseSortingOrders = new List<int>();
            for(int i = 0; i < multipleVariableSprites.Length; i++)
            {
                baseSortingOrders.Add(multipleVariableSprites[i].sortingOrder);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(!multipleSprites)
            {
                if (Vector2.Distance(collision.transform.position, frontPoint.transform.position) < Vector2.Distance(collision.transform.position, backPoint.transform.position))
                {
                    spriteRenderer.sortingOrder = baseSortingOrder - 2;
                    //collision.GetComponent<SpriteRenderer>().sortingLayerName = "PlayerFront";
                }
                else
                {
                    spriteRenderer.sortingOrder = baseSortingOrder + 2;
                    //collision.GetComponent<SpriteRenderer>().sortingLayerName = "PlayerBack";
                }
            }
            else
            {
                if(Vector2.Distance(collision.transform.position, frontPoint.transform.position) < Vector2.Distance(collision.transform.position, backPoint.transform.position))
                {
                    //collision.GetComponent<SpriteRenderer>().sortingLayerName = "PlayerFront";
                    for (int i = 0; i < multipleVariableSprites.Length; i++)
                    {
                        multipleVariableSprites[i].sortingOrder = baseSortingOrders[i] - 2;
                    }
                }
                else
                {
                    //collision.GetComponent<SpriteRenderer>().sortingLayerName = "PlayerBack";
                    for (int i = 0; i < multipleVariableSprites.Length; i++)
                    {
                        multipleVariableSprites[i].sortingOrder = baseSortingOrders[i] + 2;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //collision.GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
            if (!multipleSprites)
            {
                spriteRenderer.sortingOrder = baseSortingOrder;               
            }
            else
            {
                for (int i = 0; i < multipleVariableSprites.Length; i++)
                {
                    multipleVariableSprites[i].sortingOrder = baseSortingOrders[i];
                }
            }
        }
    }
}
