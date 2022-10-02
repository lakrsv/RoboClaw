using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _visualEffect;

    private void Awake()
    {
        _visualEffect.Stop();
    }

    public void Explode()
    {
        _visualEffect.Play();
    }
}
