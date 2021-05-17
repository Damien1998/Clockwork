using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ConveyorBelt : MonoBehaviour
{
    [SerializeField]
    private Vector2 direction;

    [SerializeField]
    private Transform itemSnapPosition;

    [SerializeField]
    private float speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.TryGetComponent(out Rigidbody2D rigidbody) && (collision.TryGetComponent(out Watch watch) || collision.TryGetComponent(out Player player)))
        {
            rigidbody.velocity = Vector2.zero;
            //rigidbody.velocity = direction.normalized * speed;
            rigidbody.AddForce(direction.normalized * speed);
            //collision.gameObject.layer = LayerMask.NameToLayer("ItemNoCollision");

            if(watch != null)
            {
                watch.ChangeSortingLayer("ItemsWorkbench");
                player = null;
                if (direction.x == 0)
                {
                    collision.transform.position = new Vector3(itemSnapPosition.position.x, collision.transform.position.y);
                }
                else if (direction.y == 0)
                {
                    collision.transform.position = new Vector3(collision.transform.position.x, itemSnapPosition.position.y);
                }
            }
            else
            {
                player = collision.GetComponent<Player>();
            }

            if(player != null)
            {
                player.isOnConveyor = true;
            }

            

        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rigidbody) && (collision.TryGetComponent(out Watch watch) || collision.TryGetComponent(out Player player)))
        {
            if(watch != null)
            {
                if (direction.x == 0)
                {
                    collision.transform.position = new Vector3(itemSnapPosition.position.x, collision.transform.position.y);
                }
                else if (direction.y == 0)
                {
                    collision.transform.position = new Vector3(collision.transform.position.x, itemSnapPosition.position.y);
                }
                rigidbody.velocity = direction.normalized * speed;
            }
            else
            {
                rigidbody.AddForce(direction.normalized * speed);
            }

                   
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.isOnConveyor = false;
        }

    }
}
