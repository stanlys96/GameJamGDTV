using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fighter : MonoBehaviour
{
    // Public fields
    public int hitpoint = 10;
    public int normalMaxHitpoint = 10;
    public int maxHitpoint = 10;
    public float pushRecoverySpeed = 0.2f;
    public int powerPlayerMaxHitpoint = 60;
    public UnityEvent onReceiveDamage;

    // Immunity
    public float immuneTime = 0.75f;
    protected float lastImmune;

    // Push
    // All fighters can ReceiveDamage / Die
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;

            GameManager.instance.ShowText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.zero, 0.5f);

            if (hitpoint <= 0) 
            {
                hitpoint = 0;
                Death();
            }
            if (gameObject.name == "Player")
            {
                if (onReceiveDamage != null)
                {
                    if (GameManager.instance.player.hitpoint <= 0) return;
                    onReceiveDamage.Invoke();
                }
            }
        }
    }

    protected virtual void Death()
    {

    }
}
