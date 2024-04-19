using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class Ladybug : MonoBehaviour, ITakeDamage
{
    [SerializeField] float _speed = 1;
    [SerializeField] float _raycastDistance;
    [SerializeField] LayerMask _forwardRaycastMask;
    
    Vector2 _direction = Vector2.left;

    SpriteRenderer _sr;
    Animator _anim;
    Collider2D _col;
    Rigidbody2D _rig;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _col = GetComponent<Collider2D>();
        _rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        CheckInFront();
        CheckGroundInFront();

        Move();
    }
    void CheckInFront()
    {
        Vector2 offset = _direction * _col.bounds.extents.x;
        Vector2 origin = (Vector2)transform.position + offset;

        var hits = Physics2D.RaycastAll(origin, _direction, _raycastDistance, _forwardRaycastMask);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != this.gameObject)
            {
                Turnaround();
                break;
            }
        }
    }

    void CheckGroundInFront()
    {
        bool canContinueWalking = false;

        var downOrigin = GetDownRayPosition(_col);
        var downHits = Physics2D.RaycastAll(downOrigin, Vector2.down, _raycastDistance);
        foreach (var hit in downHits)
        {
            if (hit.collider != null && hit.collider.gameObject != this.gameObject)
                canContinueWalking = true;
        }

        if (!canContinueWalking)
            Turnaround();
    }

    void Move()
    {
        _rig.velocity = new Vector2(_direction.x * _speed, _rig.velocity.y);
    }

    void Turnaround()
    {
        _direction *= -1;
        _sr.flipX = _direction == Vector2.right;
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<Collider2D>();
        Vector2 offset = _direction * collider.bounds.extents.x;
        Vector2 origin = (Vector2)transform.position + offset;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + (_direction * _raycastDistance));

        var downOrigin = GetDownRayPosition(collider);
        
        Gizmos.DrawLine(downOrigin, downOrigin + (Vector2.down * _raycastDistance));
    }

    Vector2 GetDownRayPosition(Collider2D collider)
    {
        var bounds = collider.bounds;

        if (_direction == Vector2.left)
            return new Vector2(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y);
        else
            return new Vector2(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y);

    }
}
