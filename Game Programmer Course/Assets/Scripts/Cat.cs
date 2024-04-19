using UnityEngine;

public class Cat : MonoBehaviour, ITakeDamage
{
    [SerializeField] Vector2 _fireDirection;
    [SerializeField] Transform _catBombFirePoint;
    [SerializeField] int _health = 7;

    CatBomb _catbomb;

    void Start()
    {
        SpawnCatBomb();

        var shootAnimationWrapper = GetComponentInChildren<ShootAnimationWrapper>();
        shootAnimationWrapper.OnShoot += ShootCatBomb;
        shootAnimationWrapper.OnReload += SpawnCatBomb;
    }

    void SpawnCatBomb()
    {
        if(_catbomb == null)
            _catbomb = PoolManager.Instance.GetCatBomb();

        _catbomb.transform.position = _catBombFirePoint.position;
        _catbomb.transform.SetParent(_catBombFirePoint);
    }

    void ShootCatBomb()
    {
        _catbomb.Launch(_fireDirection);
        _catbomb = null;
    }

    public void TakeDamage()
    {
        _health--;
        if (_health <= 0)
            Destroy(gameObject);
    }
}
