using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IBind<CoinData>
{
    CoinData _data;

    SpriteRenderer sprite;
    AudioSource audioS;
    BoxCollider2D col;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        audioS = GetComponent<AudioSource>();
        col = GetComponent<BoxCollider2D>();
    }
    public void Bind(CoinData data)
    {
        _data = data;
        if(_data.IsCollected)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player)
        {
            _data.IsCollected = true;
            player.AddPoint();
            audioS.Play();
            gameObject.SetActive(false);

        }
    }
}
