using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider> EnteredTrigger;

    void OnTriggerEnter(Collider collider)
    {
        EnteredTrigger?.Invoke(collider);
    }
}
