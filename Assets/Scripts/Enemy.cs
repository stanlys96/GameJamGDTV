using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

public class Enemy : Mover, ISaveable
{
    [System.Serializable]
    struct EnemySaveItems
    {
        public SerializableVector3 startingPosition;
        public SerializableVector3 currentPosition;
        public int hitpoint;
        public int maxHitpoint;
        public bool chasing;
        public bool collidingWithPlayer;
        public bool isDead;
    }

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
    private bool isDead = false;

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[20];

    void Awake()
    {
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
                    UpdateMotor(new Vector3(0, 0, 0), animator);
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
        isDead = true;
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        if (dropOnDead != null)
        {
            GameManager.instance.DropOnDead(dropOnDead, transform);
        }
        Destroy(gameObject);
    }

    public object CaptureState()
    {
        EnemySaveItems saveItems = new EnemySaveItems();
        saveItems.startingPosition = new SerializableVector3(startingPosition);
        saveItems.currentPosition = new SerializableVector3(transform.position);
        saveItems.hitpoint = hitpoint;
        saveItems.maxHitpoint = maxHitpoint;
        saveItems.chasing = chasing;
        saveItems.collidingWithPlayer = collidingWithPlayer;
        saveItems.isDead = isDead;
        return saveItems;
    }

    public void RestoreState(object state)
    {
        EnemySaveItems saveItems = (EnemySaveItems)state;

        this.startingPosition = saveItems.startingPosition.ToVector();
        transform.position = saveItems.currentPosition.ToVector();
        this.hitpoint = saveItems.hitpoint;
        this.maxHitpoint = saveItems.maxHitpoint;
        this.chasing = saveItems.chasing;
        this.collidingWithPlayer = saveItems.collidingWithPlayer;
        this.isDead = saveItems.isDead;
        if (this.hitpoint == 0)
        {
            Destroy(gameObject);
        }
    }
}
