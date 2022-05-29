using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using UnityEngine.UI;

public class Player : Mover, ISaveable
{
    [System.Serializable]
    struct SaveItems
    {
        public int pesos;
        public int experience;
        public int playerLives;
        public int weaponLevel;
        public int hitpoint;
        public int maxHitpoint;
        public SerializableVector3 position;
        public bool onPowerPlayer;
        public float timeSinceLastPowerPlayer;
    }

    public SpriteRenderer spriteRenderer;
    private bool isAlive = true;
    public bool onPowerPlayer = false;
    public Animator animator;
    public Vector2 savedPosition;
    public string savedScene = "Main";
    private float lastSwing;
    private float timeSinceLastPowerPlayer = Mathf.Infinity;
    private float powerPlayerDuration = 15f;
    private bool alreadySetRespawn = false;
    private float weaponCooldown = 1.0f;
    private float cooldown = 4.0f;
    private float lastShout = -4.0f;
    public float powerPlayerCounter = Mathf.Infinity;

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
        if (GameManager.instance.playerLives == 0)
        {
            GameManager.instance.deathMenuAnim.SetTrigger("Show");
            return;
        }
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
        powerPlayerCounter = 15f;
    }

    public void BeNormalPlayer()
    {
        onPowerPlayer = false;
        spriteRenderer.sprite = GameManager.instance.playerSprites[0];
        animator.runtimeAnimatorController = GameManager.instance.playerAnimators[0];
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
        maxHitpoint = normalMaxHitpoint;
        Heal(maxHitpoint, onPower: true);
        timeSinceLastPowerPlayer = Mathf.Infinity;
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
            if (Time.time - lastSwing > weaponCooldown)
            {
                lastSwing = Time.time;
                Swing();
            }
        }
        if (timeSinceLastPowerPlayer > powerPlayerDuration && onPowerPlayer)
        {
            BeNormalPlayer();
        }
        powerPlayerCounter -= Time.deltaTime;
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
        if (Time.time - lastShout > cooldown)
        {
            lastShout = Time.time;
            GameManager.instance.ShowText("Levelled Up!", 25, Color.cyan, transform.position + new Vector3(0, 0.16f, 0), Vector3.zero, cooldown);
        }
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

    public object CaptureState()
    {
        SaveItems saveItems = new SaveItems();
        saveItems.experience = GameManager.instance.experience;
        saveItems.pesos = GameManager.instance.pesos;
        saveItems.playerLives = GameManager.instance.playerLives;
        saveItems.weaponLevel = GameManager.instance.weapon.weaponLevel;
        saveItems.position = new SerializableVector3(transform.position);
        saveItems.hitpoint = hitpoint;
        saveItems.maxHitpoint = maxHitpoint;
        saveItems.onPowerPlayer = onPowerPlayer;
        saveItems.timeSinceLastPowerPlayer = timeSinceLastPowerPlayer;
        return saveItems;
    }

    public void RestoreState(object state)
    {
        SaveItems saveItems = (SaveItems)state;
        GameManager.instance.pesos = saveItems.pesos;
        GameManager.instance.experience = saveItems.experience;
        GameManager.instance.weapon.SetWeaponLevel(saveItems.weaponLevel);
        GameManager.instance.playerLives = saveItems.playerLives;
        transform.position = saveItems.position.ToVector();
        this.hitpoint = saveItems.hitpoint;
        this.maxHitpoint = saveItems.maxHitpoint;
        this.onPowerPlayer = saveItems.onPowerPlayer;
        this.timeSinceLastPowerPlayer = saveItems.timeSinceLastPowerPlayer;
    }
}
