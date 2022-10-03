using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.ObjectPool;

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
    [SerializeField]
    private float _maxVelocityMagnitude;
    [SerializeField]
    private Beam _moveBeam;
    [SerializeField]
    private Beam _attackBeam;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private ClawController _otherClaw;

    private Vector3 _parentedLocalPosition;
    private Quaternion _parentedLocalRotation;
    private Quaternion _parentedWorldRotation;
    private Transform _originalParent;

    private bool _clawLaunched = false;
    private Vector3 _initialPosition;
    private RaycastHit _hit;
    private float _initialDistance;
    private float _currentLerp;
    private GrabTarget _grabTarget;
    //private float _targetOffset = 0.13206f;

    public GrabTarget GrabTarget => _grabTarget;

    private void Awake()
    {
        // Not parented, but kind of.
        _parentedLocalPosition = transform.localPosition;
        _parentedLocalRotation = transform.localRotation;
        _parentedWorldRotation = transform.rotation;
        _originalParent = transform.parent;
        _rigidBody.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_clawLaunched)
        {
            if (Input.GetMouseButtonUp(_isRightClaw ? 1 : 0))
            {
                _lineRenderer.positionCount = 0;
                Physics.Raycast(transform.position, Camera.main.transform.forward, out RaycastHit hit, 20f);
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

                    _moveBeam.PlayBeam(_initialDistance / _speed / 4f);
                }
                else
                {
                    _clawAnimator.CloseClaw();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!_clawLaunched)
        {
            if (Input.GetMouseButton(_isRightClaw ? 1 : 0))
            {
                _clawAnimator.OpenClaw();
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, transform.position + Camera.main.transform.forward * 100);
            }
            transform.localPosition = _parentedLocalPosition;
            transform.localRotation = _parentedLocalRotation;
            _rigidBody.velocity = Vector3.zero;
        }
        else if (_grabTarget != null && transform.parent == _grabTarget.GrabChildTarget)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void FixedUpdate()
    {
        if (_clawLaunched)
        {
            if (_currentLerp == 1f)
            {
                return;
            }


            var acceleration = Mathf.Lerp(_accelerationSpeed, 0f, _motionCurve.Evaluate(_currentLerp));
            _currentLerp += ((_speed + acceleration) * Time.deltaTime) / _initialDistance;
            _currentLerp = Mathf.Clamp01(_currentLerp);

            var goTo = _grabTarget != null ? _grabTarget.GrabChildTarget.position : _hit.point;

            var newPosition = Vector3.Lerp(_initialPosition, goTo, _currentLerp);
            newPosition.y += _motionCurve.Evaluate(_currentLerp) * _maxHeightOffset;

            var velocity = newPosition - _rigidBody.position;

            var lookRotation = Quaternion.LookRotation(velocity * 10f, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
            if (_currentLerp < 0.4f)
            {
                lookRotation *= Quaternion.Euler(-25f, 0f, 0f);
            }
            else if (_currentLerp > 0.4f && _currentLerp < 0.7f)
            {
                if (_grabTarget != null)
                {
                    lookRotation = Quaternion.LookRotation(_grabTarget.GrabChildTarget.position - _rigidBody.position, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
                }
                else
                {
                    lookRotation = Quaternion.LookRotation(_hit.point - _rigidBody.position, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
                }
            }

            if (_currentLerp > 0.7f && _grabTarget == null)
            {
                _rigidBody.isKinematic = false;
            }

            _rigidBody.rotation = Quaternion.RotateTowards(_rigidBody.rotation, lookRotation, 360f * Time.deltaTime);
            _rigidBody.MovePosition(newPosition);
            //transform.position = newPosition;

            if (_currentLerp == 1f)
            {
                transform.rotation = lookRotation;

                if (_grabTarget == null || _grabTarget.IsOccupied)
                {
                    StartCoroutine(RecallClaw());

                }
                else
                {
                    transform.SetParent(_grabTarget.GrabChildTarget.transform, false);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    _clawAnimator.GrabTarget();
                    _grabTarget.IsOccupied = true;
                    _rigidBody.isKinematic = true;

                    StartCoroutine(AttackTarget());
                }

            }
        }
        _rigidBody.velocity = Vector3.ClampMagnitude(_rigidBody.velocity, _maxVelocityMagnitude);
    }

    private IEnumerator RecallClaw()
    {
        yield return new WaitForSeconds(0.5f);

        //transform.SetParent(_originalParent.transform, true);
        _clawAnimator.CloseClaw();
        _rigidBody.isKinematic = true;
        _moveBeam.PlayBeam(2f);
        while (Vector3.Distance(transform.position, _originalParent.TransformPoint(_parentedLocalPosition)) > 0.1f)
        {
            var remainingDistance = Vector3.Distance(transform.position, _originalParent.TransformPoint(_parentedLocalPosition));
            var lookRotation = Quaternion.LookRotation(Vector3.up, _originalParent.TransformPoint(_parentedLocalPosition) - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, remainingDistance > 1f ? lookRotation : Quaternion.Inverse(lookRotation), 2 * 360f * Time.deltaTime);

            if (remainingDistance < 1f)
            {
                _moveBeam.StopBeam();
            }


            transform.position = Vector3.MoveTowards(transform.position, _originalParent.TransformPoint(_parentedLocalPosition), _speed * 2 * Time.deltaTime);
            yield return null;
        }

        transform.SetParent(_originalParent);
        transform.localPosition = _parentedLocalPosition;
        transform.localRotation = _parentedLocalRotation;

        _grabTarget = null;
        _clawLaunched = false;
        _currentLerp = 0f;

        yield return null;

        _moveBeam.StopBeam();
    }

    private IEnumerator AttackTarget()
    {
        yield return new WaitForSeconds(0.5f);
        _attackBeam.PlayBeam(2.0f);
        yield return new WaitForSeconds(0.25f);
        _grabTarget.transform.root.GetComponent<MechSpider>().StartShaking();
        yield return new WaitForSeconds(1.5f);

        transform.SetParent(null);
        _rigidBody.isKinematic = false;
        _rigidBody.AddExplosionForce(1000f, _grabTarget.transform.position, 2f);

        var explosionPosition = _grabTarget.transform.position;

        var affected = Physics.SphereCastAll(_grabTarget.transform.root.position, 4f, Vector3.up, 20f, LayerMask.GetMask("Spider"));
        foreach (var body in affected)
        {
            if (body.collider.CompareTag("Spider"))
            {
                var ragdollPosition = body.transform.root.position;
                var ragdollRotation = body.transform.root.rotation;
                var ragdoll = ObjectPools.Instance.GetPooledObject<MechSpiderRagdoll>();
                if (ragdoll != null)
                {
                    ragdoll.transform.position = ragdollPosition;
                    ragdoll.transform.rotation = ragdollRotation;
                }

                if(_otherClaw.GrabTarget != null && _otherClaw.GrabTarget.transform.root == body.transform.root)
                {
                    _otherClaw.transform.SetParent(null);
                    _otherClaw.StopAllCoroutines();
                    StartCoroutine(_otherClaw.RecallClaw());
                }

                Destroy(body.transform.root.gameObject);
                if (ragdoll != null)
                {
                    StartCoroutine(ragdoll.Explode(explosionPosition));
                }

                UI.Instance.AddPlayerScore(1);
            }
        }


        yield return RecallClaw();
    }
}
