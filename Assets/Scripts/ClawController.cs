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
    [SerializeField]
    private ClawAnimator _clawAnimator;

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
                    _clawAnimator.OpenClaw();

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
            if (_currentLerp == 1f)
            {
                return;
            }

            if (_grabTarget == null || _grabTarget.IsOccupied)
            {
                _rigidBody.isKinematic = false;
            }


            var acceleration = Mathf.Lerp(_accelerationSpeed, 0f, _motionCurve.Evaluate(_currentLerp));
            _currentLerp += ((_speed + acceleration) * Time.deltaTime) / _initialDistance;
            _currentLerp = Mathf.Clamp01(_currentLerp);

            var goTo = _grabTarget != null ? _grabTarget.GrabChildTarget.position : _hit.point;

            var newPosition = Vector3.Lerp(_initialPosition, goTo, _currentLerp);
            newPosition.y += _motionCurve.Evaluate(_currentLerp) * _maxHeightOffset;

            var velocity = newPosition - transform.position;

            var lookRotation = Quaternion.LookRotation(Vector3.up, newPosition - transform.position);

            if (_currentLerp > 0.8f && _grabTarget != null)
            {
                lookRotation = Quaternion.LookRotation(Vector3.up, _grabTarget.GrabChildTarget.position - transform.position);
            }

            transform.rotation = lookRotation;
            transform.position = newPosition;

            if (_currentLerp == 1f)
            {
                transform.rotation = lookRotation;

                if (_grabTarget == null || _grabTarget.IsOccupied)
                {
                    // TODO - Enable rigidbody, the claw falls!
                    _rigidBody.isKinematic = false;
                    _rigidBody.velocity = velocity;

                    // TODO - Recall routine

                }
                else
                {
                    transform.SetParent(_grabTarget.transform, false);
                    transform.localPosition = _grabTarget.GrabChildTarget.localPosition;
                    transform.localRotation = _grabTarget.GrabChildTarget.localRotation;
                    _clawAnimator.GrabTarget();
                    _grabTarget.IsOccupied = true;
                    _rigidBody.isKinematic = true;
                }

            }
        }
    }
}
