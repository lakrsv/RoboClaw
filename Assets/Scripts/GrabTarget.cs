using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTarget : MonoBehaviour
{
    [SerializeField]
    private Transform _grabTarget;

    public bool IsOccupied = false;

    public Transform GrabChildTarget { get { return _grabTarget; } }
}
