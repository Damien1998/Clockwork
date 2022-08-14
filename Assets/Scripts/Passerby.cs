using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passerby : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Animator animator;
    public SpriteRenderer spriteRenderer;
    public ParticleSystem clickFX;

    public float direction;
    public float speed;

    private bool isJumpingUp;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if(direction < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isJumpingUp)
        {
            rb2D.velocity = new Vector2(direction * speed, 0);
        }
        else
        {
            rb2D.velocity = Vector2.zero;
        }

    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButton(0) && !isJumpingUp)
        {
            animator.Play("JumpUp");
            isJumpingUp = true;
            direction = -direction;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            clickFX.Play();
        }
    }

    public void LeaveJumpState()
    {
        isJumpingUp = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
