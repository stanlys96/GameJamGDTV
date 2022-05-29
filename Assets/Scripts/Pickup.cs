using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupCategory
{
    Heart,
    Coin
}

public class Pickup : MonoBehaviour
{
    public PickupCategory pickupCategory;
    public int value = 0;
    public float cooldown = 4.0f;
    public float lastShout = -4.0f;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if (pickupCategory == PickupCategory.Coin)
            {
                GameManager.instance.pesos += value;
                GameManager.instance.ShowText("+ " + value + " pesos!", 25, Color.yellow, transform.position, Vector3.up * 25, 1.5f);
                Destroy(gameObject);
            } else
            {
                if (pickupCategory == PickupCategory.Heart)
                {
                    if (GameManager.instance.playerLives < 3)
                    {
                        GameManager.instance.livesContainer.transform.GetChild(GameManager.instance.playerLives).gameObject.SetActive(true);
                        GameManager.instance.playerLives++;
                        GameManager.instance.ShowText("+ " + 1 + " life!", 25, Color.green, transform.position, Vector3.up * 25, 1.5f);
                        Destroy(gameObject);
                    }
                    else
                    {
                        if (Time.time - lastShout > cooldown)
                        {
                            lastShout = Time.time;
                            GameManager.instance.ShowText("You can only have maximum 3 lives!", 25, Color.white, transform.position + new Vector3(0, 0.16f, 0), Vector3.zero, cooldown);
                        }
                    }
                }
            }
        }
    }
}
