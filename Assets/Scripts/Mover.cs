using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : Fighter
{
    private Vector3 originalSize;
    private Vector3 healthBarOriginalSize;
    protected BoxCollider2D boxCollider;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    public float ySpeed = 0.75f;
    public float xSpeed = 1.0f;
    public Rigidbody2D rigidBody;
    public GameObject healthBar;

    protected virtual void Start() 
    {
        originalSize = transform.localScale;
        boxCollider = GetComponent<BoxCollider2D>();
        if (healthBar != null)
        {
            healthBarOriginalSize = healthBar.transform.localScale;
        }
    }

    protected virtual void UpdateMotor(Vector3 input, Animator animator)
    {
        bool alreadySet = false;
        bool alreadySetRB = false;
        // Reset MoveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, input.z);

        // Swap sprite direction, whether you're going right or left
        if (moveDelta.x > 0) {
            transform.localScale = originalSize;
            if (healthBar != null)
            {
                healthBar.transform.localScale = healthBarOriginalSize;
            }
        } else if (moveDelta.x < 0) {
            transform.localScale = new Vector3(originalSize.x * -1, originalSize.y, originalSize.z);
            if (healthBar != null)
            {
                healthBar.transform.localScale = new Vector3(healthBarOriginalSize.x * -1, healthBarOriginalSize.y, healthBarOriginalSize.z);
            }
        }

        // Add push vector, if any

        // Reduce push force every frame, based off recovery speed

        // Make sure we can move in this direction, by casting a box there first, if the box returns null, we're free to move
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        rigidBody.velocity = new Vector2(moveDelta.x * Time.deltaTime * 40, moveDelta.y * Time.deltaTime * 40);
        if (!Mathf.Approximately(input.y, 0))
        {
            alreadySetRB = true;
        }


        if (animator != null)
        {
            animator.SetFloat("speed", Mathf.Abs(input.y));
            if (!Mathf.Approximately(input.y, 0))
            {
                alreadySet = true;
            }
        }

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));

        if (animator != null)
        {
            if (!alreadySet)
            {
                animator.SetFloat("speed", Mathf.Abs(input.x));
            }
        }
    }
}
