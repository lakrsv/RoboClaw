using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamBox : MonoBehaviour
{
    [SerializeField]
    private MechSpider _mechSpider;

    private bool _playerInBox;

    public float PlayerHealth = 100f;

    private void Update()
    {
        if(_playerInBox && _mechSpider.IsBeaming)
        {
            UI.Instance.DecrementPlayerHealth(Time.deltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInBox = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInBox = false;
        }
    }
}
