using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CircleCollider2D))]
public class WarpHole : MonoBehaviour
{
    public Transform exit;

    public float delay;

    public void Teleport(Player player)
    {
        Debug.Log("AAAAAAAAA");
        StartCoroutine(WaitAndTeleport(player));
    }

    IEnumerator WaitAndTeleport(Player player)
    {
        player.lockMovement = true;
        yield return new WaitForSeconds(delay);
        player.transform.position = exit.transform.position;
        player.lockMovement = false;
    }
}
