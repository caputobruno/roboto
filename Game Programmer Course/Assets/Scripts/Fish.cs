using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    [SerializeField] SplineAnimate _splineAnimate;
    [SerializeField] Animator _anim;
    [SerializeField] SplineAttackPoints _slpineAttackPoints;
    [SerializeField] int _spikeCount = 5;
    [SerializeField] int _spread = 15;
    [SerializeField] int _origin = 0;
    [SerializeField] float _fireSpeed = 5;

    float _nextAttakPoint;
    Queue<float> _attackPoints;

    private void Start()
    {
        GetComponentInChildren<ShootAnimationWrapper>().OnShoot += ShootSpikes;
        RefreshAttackPoints();
    }

    private void Update()
    {
        var elapsed = _splineAnimate.NormalizedTime % 1;
        if(elapsed >= _nextAttakPoint)
        {
            _anim.SetTrigger("attack");

            if (_attackPoints.Any())
                _nextAttakPoint = _attackPoints.Dequeue();
            else
                RefreshAttackPoints();
        }
    }

    private void RefreshAttackPoints()
    {
        _attackPoints = _slpineAttackPoints.GetAsQueue();
        _nextAttakPoint = _attackPoints.Dequeue();
    }

    void ShootSpikes()
    {
        for(int i = 0; i < _spikeCount; i++)
        {
            var angle = i - (_spikeCount / 2);
            var offset = _spread * angle;
            var finalAngle = _origin + offset;
            var spike = PoolManager.Instance.GetSpike();
            spike.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, finalAngle));
            spike.GetComponent<Rigidbody2D>().velocity = spike.transform.right * _fireSpeed;
        }
    }
}
