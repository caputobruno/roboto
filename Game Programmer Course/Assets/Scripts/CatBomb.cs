using UnityEngine;
using UnityEngine.Pool;

public class CatBomb : MonoBehaviour
{
    [SerializeField] float _launchForce;
    [SerializeField] float _selfDestructTime;

    float _selfDestructLimit;
    bool _exploded = false;
    bool _launched = false;

    Rigidbody2D _rig;
    Animator _anim;
    ObjectPool<CatBomb> _pool;

    private void Awake()
    {
        _rig = GetComponent<Rigidbody2D>();
        _rig.simulated = false;
        _anim = GetComponentInChildren<Animator>();
        _anim.enabled = false;
    }

    private void OnEnable()
    {
        _rig.simulated = false;
        _anim.enabled = false;
    }

    private void Update()
    {
        if(Time.time > _selfDestructLimit && _launched)
            SelfDestruct();
    }

    public void Launch(Vector2 direction)
    {
        transform.SetParent(null);
        _rig.simulated = true;
        _anim.enabled = true;
        _rig.AddForce(direction * _launchForce);
        _selfDestructLimit = Time.time + _selfDestructTime;
        _exploded = false;
        _launched = true;
    }

    void SelfDestruct()
    {
        gameObject.SetActive(false);
        _pool.Release(this);

        _exploded = true;
        _launched = false;

        _rig.simulated = false;
        _anim.enabled = false;
    }

    public void SetPool(ObjectPool<CatBomb> pool) => _pool = pool;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out var player))
            player.TakeDamage(collision.contacts[0].normal);

        if (_exploded) return;

        PoolManager.Instance.GetExplosion(transform.position);
        SelfDestruct();
    }
}
