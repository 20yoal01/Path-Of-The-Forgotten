using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    private float activeTime = 0.1f;
    private float timeActivated;


    private float alpha;
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.7f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerSpriteRenderer;
    private Color color;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        transform.position = player.position;

        transform.localScale = player.localScale;
        timeActivated = Time.time;
    }

    private void FixedUpdate()
    {
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        spriteRenderer.color = color;

        if(Time.time >= (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
