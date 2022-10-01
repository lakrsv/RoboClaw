using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ClawAnimator : MonoBehaviour
{
    [SerializeField]
    private bool _offsetIdleAnimation;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_offsetIdleAnimation)
        {
            _animator.Play("Idle", 0, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
