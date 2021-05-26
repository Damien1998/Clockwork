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

    [SerializeField]
    private bool changeDirection;
    [SerializeField]
    private float directionChangeTime;

    private void Start()
    {
        if (changeDirection)
        {
            StartCoroutine(Platform());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.TryGetComponent(out Rigidbody2D rigidbody) && (collision.TryGetComponent(out Watch watch) || collision.TryGetComponent(out Player player) || collision.CompareTag("water")))
        {
            
            //rigidbody.velocity = direction.normalized * speed;
            
            //collision.gameObject.layer = LayerMask.NameToLayer("ItemNoCollision");

            if(watch != null)
            {
                rigidbody.velocity = Vector2.zero;
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
            else if(collision.CompareTag("water"))
            {
                rigidbody.velocity = Vector2.zero;
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
                //rigidbody.AddForce(direction.normalized * speed, ForceMode2D.Impulse);
                player.additionalVelocity = direction.normalized * speed;
            }

            

        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rigidbody) && (collision.TryGetComponent(out Watch watch) || collision.TryGetComponent(out Player player) || collision.CompareTag("water")))
        {
            if(watch != null || collision.CompareTag("water"))
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
                player = null;
            }
            else
            {
                player = collision.GetComponent<Player>();
            }
            if (player != null)
            {
                player.additionalVelocity = direction.normalized * speed;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.isOnConveyor = false;
            player.additionalVelocity = Vector2.zero;
        }

    }
    IEnumerator Platform()
    {
        while (true)
        {
            yield return new WaitForSeconds(directionChangeTime);
            Debug.LogError("adsASDSADASDA");
            direction = new Vector2(direction.x * -1f, direction.y * -1f);
        }
    }
}
