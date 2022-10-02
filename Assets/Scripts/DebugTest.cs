using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.ObjectPool;

public class DebugTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var ragdoll = ObjectPools.Instance.GetPooledObject<MechSpiderRagdoll>();
            if (ragdoll != null)
            {
                ragdoll.transform.position = transform.position;
                StartCoroutine(ragdoll.Explode());
            }
        }
    }
}
