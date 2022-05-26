using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    public SpriteRenderer spriteRenderer;
    private bool isAlive = true;
    public bool onPowerPlayer = false;
    public Animator animator;
    public Vector2 savedPosition;
    public string savedScene = "Main";
    private float cooldown = 1f;
    private float lastSwing;
    private float timeSinceLastPowerPlayer = Mathf.Infinity;
    private float powerPlayerDuration = 30f;

    protected override void Start()
    {
        base.Start();
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive) return;
        base.ReceiveDamage(dmg);
        GameManager.instance.OnHitpointChange();
    }

    protected override void Death()
    {
        // isAlive = false;
        // GameManager.instance.deathMenuAnim.SetTrigger("Show");
        BePowerPlayer();
        GameManager.instance.playerLives--;
        GameManager.instance.livesContainer.transform.GetChild(GameManager.instance.playerLives).gameObject.SetActive(false);
    }

    private void BePowerPlayer()
    {
        spriteRenderer.sprite = GameManager.instance.powerPlayerSprite;
        animator.runtimeAnimatorController = GameManager.instance.powerPlayerAnimatorController;
        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
        maxHitpoint = powerPlayerMaxHitpoint;
        GetComponent<CapsuleCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = false;
        Heal(maxHitpoint);
        onPowerPlayer = true;
        timeSinceLastPowerPlayer = 0f;
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (isAlive)
        {
            UpdateMotor(new Vector3(x, y, 0), animator);
        }
    }

    private void Update()
    {
        timeSinceLastPowerPlayer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastSwing > cooldown)
            {
                lastSwing = Time.time;
                Swing();
            }
        }
        if (timeSinceLastPowerPlayer > powerPlayerDuration && onPowerPlayer)
        {
            onPowerPlayer = false;
            spriteRenderer.sprite = GameManager.instance.playerSprites[0];
            animator.runtimeAnimatorController = GameManager.instance.playerAnimators[0];
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = true;
            gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
            maxHitpoint = normalMaxHitpoint;
            Heal(maxHitpoint, onPower: true);
        }
    }

    public void SwapSprite(int skinId)
    {
        if (onPowerPlayer) return;
        spriteRenderer.sprite = GameManager.instance.playerSprites[skinId];
        animator.runtimeAnimatorController = GameManager.instance.playerAnimators[skinId];
    }

    private void Swing()
    {
        animator.SetTrigger("Swing");
    }

    public void OnLevelUp()
    {
        maxHitpoint++;
        powerPlayerMaxHitpoint++;
        normalMaxHitpoint++;
        hitpoint = maxHitpoint;
    }

    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
        {
            OnLevelUp();
        }
    }

    public void Heal(int healingAmount, bool onPower = false)
    {
        if (hitpoint == maxHitpoint && !onPower) return;

        hitpoint += healingAmount;
        if (hitpoint > maxHitpoint)
        {
            hitpoint = maxHitpoint;
        }
        GameManager.instance.ShowText("+" + healingAmount.ToString() + " hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.instance.OnHitpointChange();
    }

    public void Respawn()
    {
        Heal(maxHitpoint);
        isAlive = true;
        lastImmune = Time.time;
    }
}
