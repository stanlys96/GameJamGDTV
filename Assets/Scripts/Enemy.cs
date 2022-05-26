using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // Experience
    public int xpValue = 1;

    // Logic
    public float triggerLength = 1f;
    public float chaseLength = 5f;
    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPosition;
    public Animator animator;
    public GameObject dropOnDead;

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[20];

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        // Is the player in range?
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength) 
            {
                chasing = true;
            }
              
            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized, animator);
                    animator.SetFloat("speed", 1f);
                }
                else
                {
                    animator.SetFloat("speed", 0f);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position, animator);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position, animator);
            animator.SetFloat("speed", 1f);
            chasing = false;
        }

        if (transform.position == startingPosition)
        {
            animator.SetFloat("speed", 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collidingWithPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collidingWithPlayer = false;
        }
    }

    protected override void Death()
    {
        if (dropOnDead != null)
        {
            Instantiate(dropOnDead, transform);
        }
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        if (dropOnDead != null)
        {
            GameManager.instance.DropOnDead(dropOnDead, transform);
        }
        Destroy(gameObject);
    }
}
