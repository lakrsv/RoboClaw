using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private CharacterController _controller;

    // Update is called once per frame
    void LateUpdate()
    {
        _animator.SetFloat("Speed", _controller.velocity.magnitude);
    }
}
