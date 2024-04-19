using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
   [SerializeField]  BlasterShot _blasterShootPrefab;
   [SerializeField] ReturnToPool _explosionPrefab;
   [SerializeField] CatBomb _catBombPrefab;
   [SerializeField] ReturnToPool _spikeFishPrefab;

    ObjectPool<BlasterShot> _blasterShotPool;
    ObjectPool<ReturnToPool> _blasterExplosionPool;
    ObjectPool<CatBomb> _catBombPool;
    ObjectPool<ReturnToPool> _spikePool;

    public static PoolManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        _blasterShotPool = new ObjectPool<BlasterShot>(
            () =>
            {
                var shot = Instantiate(_blasterShootPrefab);
                shot.SetPool(_blasterShotPool);
                return shot;
            },
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));

        _blasterExplosionPool = new ObjectPool<ReturnToPool>(
            () =>
            {
                var shot = Instantiate(_explosionPrefab);
                shot.SetPool(_blasterExplosionPool);
                return shot;
                },
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));

        _catBombPool = new ObjectPool<CatBomb>(
            () => {
                var catBomb = Instantiate(_catBombPrefab);
                catBomb.SetPool(_catBombPool);
                return catBomb;
            },
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));

        _spikePool = new ObjectPool<ReturnToPool>(
            () =>
            {
                var shot = Instantiate(_spikeFishPrefab);
                shot.SetPool(_spikePool);
                return shot;
                },
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));
    }

    public BlasterShot GetBlasterShot() => _blasterShotPool.Get();

    public CatBomb GetCatBomb() => _catBombPool.Get();

    public ReturnToPool GetExplosion(Vector2 point)
    {
        var explosion = _blasterExplosionPool.Get();
        explosion.transform.position = point;
        return explosion;
    }

    public ReturnToPool GetSpike() => _spikePool.Get();
}
