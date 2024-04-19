using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Frog : MonoBehaviour, ITakeDamage
{
    Rigidbody2D rig;
    SpriteRenderer sprite;
    AudioSource audioSource;

    [SerializeField] float _minJumpDelay;
    [SerializeField] float _maxJumpDelay;
    [SerializeField] Vector2 jumpForce;
    [SerializeField] Sprite jumpSprite;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int numberOfJumps;

    float _jumpDelay;
    int jumpsLeft;

    Sprite defaultSprite;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        defaultSprite = sprite.sprite;
        jumpsLeft = numberOfJumps;

        StartCoroutine(Jump());
    }

    IEnumerator Jump()
    {
        if(jumpsLeft == 0)
        {
            jumpsLeft = numberOfJumps;
            jumpForce *= new Vector2(-1, 1);
            sprite.flipX = !sprite.flipX;
        }

        rig.velocity = jumpForce;
        sprite.sprite = jumpSprite;
        audioSource.Play();
        jumpsLeft--;

        _jumpDelay = Random.Range(_minJumpDelay, _maxJumpDelay);

        yield return new WaitForSeconds(_jumpDelay);

        StartCoroutine(Jump());
    }

    public void TakeDamage()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(collision.gameObject.layer == groundLayer)
            sprite.sprite = defaultSprite;
    }
}
