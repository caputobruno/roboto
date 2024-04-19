using UnityEngine;
using UnityEngine.Pool;

public class Blaster : Item
{
    [SerializeField] Transform _firePoint;
    [SerializeField] float _cooldownTime;

    Player _player;

    private void Awake()
    {

        _player = GetComponentInParent<Player>();
    }

    void Fire()
    {
        BlasterShot blasterShot = PoolManager.Instance.GetBlasterShot();
        blasterShot.Launch(_player.Direction, _firePoint.position);
    }

    public override void Use()
    {
        if (GameManager.CinematicPlaying) return;

        Fire();
    }
}
