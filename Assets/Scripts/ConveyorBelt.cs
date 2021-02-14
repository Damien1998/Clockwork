using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ConveyorBelt : MonoBehaviour
{
    [SerializeField]
    private Vector2 direction;

    [SerializeField]
    private Transform itemSnapPostition;

    [SerializeField]
    private float speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = direction.normalized * speed;
            if(direction.x == 0)
            {
                collision.transform.position = new Vector3(itemSnapPostition.position.x, collision.transform.position.y);
            }
            else if (direction.y == 0)
            {
                collision.transform.position = new Vector3(collision.transform.position.x, itemSnapPostition.position.y);
            }

        }
        
    }
}
