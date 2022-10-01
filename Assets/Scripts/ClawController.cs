using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawController : MonoBehaviour
{
    [SerializeField]
    private bool _isRightClaw;
    [SerializeField]
    private AnimationCurve _motionCurve;
    [SerializeField]
    private float _maxHeightOffset;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _accelerationSpeed;
    [SerializeField]
    private Rigidbody _rigidBody;

    private Vector3 _parentedLocalPosition;
    private Quaternion _parentedLocalRotation;
    private Transform _originalParent;

    private bool _clawLaunched = false;
    private Vector3 _initialPosition;
    private RaycastHit _hit;
    private float _initialDistance;
    private float _currentLerp;
    private GrabTarget _grabTarget;
    //private float _targetOffset = 0.13206f;

    private void Awake()
    {
        // Not parented, but kind of.
        _parentedLocalPosition = transform.localPosition;
        _parentedLocalRotation = transform.localRotation;
        _originalParent = transform.parent;
        _rigidBody.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_clawLaunched)
        {
            if (Input.GetMouseButtonDown(_isRightClaw ? 1 : 0))
            {
                Physics.Raycast(transform.position, Camera.main.transform.forward, out RaycastHit hit);
                //Debug.DrawLine(transform.position, hit.point, Color.green, 1.0f);
                if (hit.collider != null)
                {
                    _hit = hit;
                    _clawLaunched = true;
                    _initialPosition = transform.position;
                    _initialDistance = Vector3.Distance(transform.position, hit.point);
                    transform.SetParent(null);

                    if (hit.collider.CompareTag("GrabTarget"))
                    {
                        _grabTarget = hit.collider.GetComponent<GrabTarget>();
                    }
                }
            }
        }
        else
        {

            var acceleration = Mathf.Lerp(_accelerationSpeed, 0f, _motionCurve.Evaluate(_currentLerp));
            _currentLerp += ((_speed + acceleration) * Time.deltaTime) / _initialDistance;
            _currentLerp = Mathf.Clamp01(_currentLerp);

            var newPosition = Vector3.Lerp(_initialPosition, _hit.point, _currentLerp);
            newPosition.y += _motionCurve.Evaluate(_currentLerp) * _maxHeightOffset;

            var velocity = newPosition - transform.position;

            var lookRotation = Quaternion.LookRotation(Vector3.up, newPosition - transform.position);
            transform.rotation = lookRotation;
            transform.position = newPosition;

            if (_currentLerp == 1f)
            {
                transform.rotation = lookRotation;
                _clawLaunched = false;
                // TODO - Recall

                if (_grabTarget == null)
                {
                    // TODO - Enable rigidbody, the claw falls!
                    _rigidBody.isKinematic = false;
                    _rigidBody.velocity = velocity;
                }

            }
        }
    }
}
