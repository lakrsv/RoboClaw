using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.ObjectPool;

public class MechSpiderRagdoll : MonoBehaviour, IPoolable
{
    private Rigidbody[] _rigidBodies;
    private Vector3[] _originalPositions;
    private Quaternion[] _originalRotations;
    [SerializeField]
    private Explosion _explosion;

    public bool IsEnabled => gameObject.activeInHierarchy;

    private void Awake()
    {
    }

    public IEnumerator Explode()
    {
        _explosion.Explode();
        yield return null;

        foreach (var body in _rigidBodies)
        {
            body.constraints = RigidbodyConstraints.None;
            body.AddExplosionForce(500f, transform.position, 2f, 0.25f);
        }
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        Invoke(nameof(Disable), 5.0f);
    }

    public void Disable()
    {
        if (_rigidBodies == null)
        {
            _rigidBodies = GetComponentsInChildren<Rigidbody>();
            _originalPositions = new Vector3[_rigidBodies.Length];
            _originalRotations = new Quaternion[_rigidBodies.Length];
            for (int i = 0; i < _rigidBodies.Length; ++i)
            {
                _rigidBodies[i].constraints = RigidbodyConstraints.FreezeAll;
                _originalPositions[i] = _rigidBodies[i].transform.localPosition;
                _originalRotations[i] = _rigidBodies[i].transform.localRotation;
            }
        }
        for (int i = 0; i < _rigidBodies.Length; ++i)
        {
            _rigidBodies[i].constraints = RigidbodyConstraints.FreezeAll;
            _rigidBodies[i].velocity = Vector3.zero;
            _rigidBodies[i].transform.localPosition = _originalPositions[i];
            _rigidBodies[i].transform.localRotation = _originalRotations[i];
        }
        gameObject.SetActive(false);
    }
}
