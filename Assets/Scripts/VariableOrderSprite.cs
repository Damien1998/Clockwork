using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableOrderSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private int baseSortingOrder;

    [SerializeField] private Transform frontPoint, backPoint;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseSortingOrder = spriteRenderer.sortingOrder;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(Vector2.Distance(collision.transform.position, frontPoint.transform.position) < Vector2.Distance(collision.transform.position, backPoint.transform.position))
            {
                spriteRenderer.sortingOrder = baseSortingOrder - 2;
                collision.GetComponent<SpriteRenderer>().sortingLayerName = "PlayerFront";
            }
            else
            {
                spriteRenderer.sortingOrder = baseSortingOrder + 2;
                collision.GetComponent<SpriteRenderer>().sortingLayerName = "PlayerBack";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            spriteRenderer.sortingOrder = baseSortingOrder;
            collision.GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
        }
    }
}
