using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawRope : MonoBehaviour
{
    [SerializeField]
    private Transform _targetAttachPoint;
    [SerializeField]
    private Transform _ropeTarget;

    private Quaternion _offsetRotation;
    private Vector3 _offsetPosition;

    private void Awake()
    {
        _offsetPosition = _targetAttachPoint.transform.position - _ropeTarget.transform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        _ropeTarget.transform.position = _targetAttachPoint.transform.position + _offsetPosition;
        //_ropeTarget.transform.rotation = _targetAttachPoint.transform.rotation * _offsetRotation;
    }
}
