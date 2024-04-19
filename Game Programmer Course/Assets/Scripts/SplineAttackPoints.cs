using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineAttackPoints : MonoBehaviour
{
    [SerializeField] SplineContainer _splineCointainer;
    [SerializeField] List<float> _attackPoints;

    public Queue<float> GetAsQueue()
    {
        return new Queue<float>(_attackPoints);
    }

    private void OnDrawGizmos()
    {
        foreach(var point in _attackPoints)
        {
            Vector3 position = _splineCointainer.EvaluatePosition(point);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, 0.2f);
        }
    }
}
