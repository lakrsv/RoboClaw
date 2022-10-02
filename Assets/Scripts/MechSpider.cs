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

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent.speed = _walkSpeed;
        _agent.angularSpeed = _turningSpeed;
    }

    private void Start()
    {
        StartCoroutine(UpdatePlayerTracking());
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Physics.Raycast(transform.position + Vector3.up * 0.25f, _player.position - transform.position, out RaycastHit hit, 100f, ~LayerMask.NameToLayer("Spider"));
        Debug.DrawLine(transform.position, hit.point, Color.red, 1.0f);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            _lastKnownPlayerPosition = hit.collider.transform.position;
        }
    }

    private void Update()
    {
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    private IEnumerator UpdatePlayerTracking()
    {
        while (gameObject.activeInHierarchy)
        {
            if (_lastKnownPlayerPosition.HasValue)
            {
                var directionToPlayer = (_lastKnownPlayerPosition.Value - transform.position).normalized;
                var offsetPosition = _lastKnownPlayerPosition.Value - (directionToPlayer * _keepDistanceThreshold);
                _agent.SetDestination(offsetPosition);
            }
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }
}
