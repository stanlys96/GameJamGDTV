using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            GameManager.instance.livesContainer.transform.GetChild(GameManager.instance.playerLives).gameObject.SetActive(true);
            GameManager.instance.playerLives++;
            Destroy(gameObject);
        }
    }
}
