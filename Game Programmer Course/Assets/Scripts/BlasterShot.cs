using UnityEngine;
using UnityEngine.Pool;

public class BlasterShot : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _lifetime;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] LayerMask _groundLayer;

    float _selfDestructiveTime;
    bool _exploded;

    Rigidbody2D _rig;
    ObjectPool<BlasterShot> _pool;

    private void Awake()
    {
        _rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.time >= _selfDestructiveTime)
            SelfDestruct();
    }

    private void SelfDestruct()
    {
        gameObject.SetActive(false);
        _pool.Release(this);
        _exploded = true;
    }

    public void Launch(Vector2 direction, Vector3 position)
    {
        transform.position = position;

        _rig.velocity = direction * _speed;

        if (_rig.velocity.x < 0 && transform.localScale.x > 0)
            transform.localScale *= -1;
        if (_rig.velocity.x > 0 && transform.localScale.x < 0)
            transform.localScale *= -1;

        _selfDestructiveTime = Time.time + _lifetime;

        _exploded = false;
    }
    public void SetPool(ObjectPool<BlasterShot> pool) => _pool = pool;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var damageable = collision.gameObject.GetComponent<ITakeDamage>();

        if (damageable == null && collision.gameObject.layer != _groundLayer)
            damageable = collision.gameObject.GetComponentInParent<ITakeDamage>();

        damageable?.TakeDamage();

        if (_exploded) return;

        PoolManager.Instance.GetExplosion(collision.contacts[0].point);
        
        SelfDestruct();
    }
}
