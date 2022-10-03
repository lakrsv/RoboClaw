using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechSpider : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private float _turningSpeed;
    [SerializeField]
    private float _turretTurningSpeed;
    [SerializeField]
    private float _keepDistanceThreshold;

    private Transform _player;
    private Vector3? _lastKnownPlayerPosition;

    [SerializeField]
    private Beam _beamOne;
    [SerializeField]
    private Beam _beamTwo;

    private bool _isShaking = false;
    private float _lastBeamTime;
    private bool _isBeaming;

    public bool IsBeaming => _isBeaming;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _isBeaming = false;
    }

    public void SetStats(float walkSpeed, float turnSpeed)
    {
        _walkSpeed = walkSpeed;
        _turningSpeed = turnSpeed;
        _turretTurningSpeed = turnSpeed;
        _agent.speed = walkSpeed;
        _agent.angularSpeed = turnSpeed;
    }

    public IEnumerator StartSpawning()
    {
        _rigidbody.isKinematic = false;
        _agent.enabled = false;

        yield return new WaitForSeconds(2.0f);

        _rigidbody.isKinematic = true;
        _agent.enabled = true;

        StartCoroutine(UpdatePlayerTracking());
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!_agent.enabled)
        {
            return;
        }
        Physics.Raycast(transform.position + Vector3.up * 0.25f, _player.position - transform.position, out RaycastHit hit, 100f, ~LayerMask.NameToLayer("Spider"));
        Debug.DrawLine(transform.position, hit.point, Color.red, 1.0f);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            _lastKnownPlayerPosition = hit.collider.transform.position;
        }
    }

    private void Update()
    {
        if (!_agent.enabled)
        {
            return;
        }
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    private IEnumerator UpdatePlayerTracking()
    {
        while (gameObject.activeInHierarchy)
        {
            if (_lastKnownPlayerPosition.HasValue)
            {
                var distance = Vector3.Distance(_lastKnownPlayerPosition.Value, transform.position);
                if (distance < 4f)
                {
                    var timePassed = Time.time - _lastBeamTime;
                    if (timePassed > 6f)
                    {
                        _isBeaming = true;
                        _beamOne.PlayBeam(3f);
                        _beamTwo.PlayBeam(3f);
                        StartCoroutine(DisableBeaming());
                        _lastBeamTime = Time.time;
                    }
                }

                if (distance > _keepDistanceThreshold + 0.5f)
                {
                    var directionToPlayer = (_lastKnownPlayerPosition.Value - transform.position).normalized;
                    var offsetPosition = _lastKnownPlayerPosition.Value - (directionToPlayer * _keepDistanceThreshold);
                    _agent.SetDestination(_lastKnownPlayerPosition.Value);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }

    private IEnumerator DisableBeaming()
    {
        yield return new WaitForSeconds(3.0f);
        _isBeaming = false;
    }

    public void StartShaking()
    {
        _isShaking = true;
        _animator.SetTrigger("Shake");
    }
}
