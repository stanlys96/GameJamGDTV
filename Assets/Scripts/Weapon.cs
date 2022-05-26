using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    // Damage struct
    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7 };
    public float[] pushForce = { 2.0f, 2.2f, 2.5f, 3f, 3.2f, 3.6f, 4f };
    public int[] damagePointPowerPlayer = { 7, 10, 13, 16, 19, 22, 25 };

    // Upgrade
    public int weaponLevel = 0;
    public SpriteRenderer spriteRenderer;

    // Swing
    private Animator anim;
    private float cooldown = 1f;
    private float lastSwing;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastSwing > cooldown) 
            {
                lastSwing = Time.time;
                Swing();
            }
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter") 
        {
            if (coll.name == "Player") return;

            int damageAmount = 0;

            // Create a new damage object, then we'll send it to the fighter we've hit
            if (gameObject.transform.parent.name == "Player" && GameManager.instance.player.onPowerPlayer)
            {
                damageAmount = damagePointPowerPlayer[weaponLevel];
            } 
            else
            {
                damageAmount = damagePoint[weaponLevel];
            }
            Damage dmg = new Damage
            {
                damageAmount = damageAmount,
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };

            coll.gameObject.SendMessage("ReceiveDamage", dmg);
        }
    }

    private void Swing()
    {
        anim.SetTrigger("Swing");
    }

    public void UpgradeWeapon()
    {
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }

    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }
}
