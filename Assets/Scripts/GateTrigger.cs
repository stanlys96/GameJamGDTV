using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite doorClosed;
    public Sprite doorOpen;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = doorClosed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            spriteRenderer.sprite = doorOpen;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            spriteRenderer.sprite = doorClosed;
        }
    }
}
