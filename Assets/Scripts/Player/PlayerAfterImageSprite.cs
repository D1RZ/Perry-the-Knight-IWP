using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    [SerializeField]
    private float alphaMultiplier = 0.85f;

    private Transform Player;

    private SpriteRenderer SR;
    private SpriteRenderer PlayerSR;

    private Color color;

    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>(); // this is to get the sprite renderer of the afterimage game object
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        PlayerSR = Player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = PlayerSR.sprite;
        transform.position = Player.position;
        transform.rotation = Player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1f,1f,1f,alpha);
        SR.color = color;

        if(Time.time > (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
