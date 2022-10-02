using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Beam : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _beamEffect;

    private void Awake()
    {
        _beamEffect.Stop();
    }

    public void PlayBeam(float duration)
    {
        _beamEffect.SetFloat("Duration", duration);
        _beamEffect.Play();
    }
}
