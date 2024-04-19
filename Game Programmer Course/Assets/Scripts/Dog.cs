using System;
using System.Collections;
using UnityEngine;

public class Dog : MonoBehaviour, ITakeDamage
{
    [SerializeField] GameObject _blaster;
    [SerializeField] int _health = 7;

    bool _isAttacking;

    Animator _anim;

    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        GetComponentInChildren<ShootAnimationWrapper>().OnShoot += Shoot;
        GetComponentInChildren<ShootAnimationWrapper>().OnReload += Reload;
    }

    private void Reload()
    {
        if(!_isAttacking)
            _anim.SetTrigger("attack");
    }

    void Shoot()
    {
        _blaster.SetActive(true);
        _isAttacking = true;
        StartCoroutine(StopShooting());
    }

    IEnumerator StopShooting()
    {
        yield return new WaitForSeconds(1);
        _blaster.SetActive(false);
        _isAttacking = false;
    }

    public void TakeDamage()
    {
        _health--;
        if(_health <= 0)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
