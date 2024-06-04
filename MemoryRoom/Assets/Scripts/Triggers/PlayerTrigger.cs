using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public CustomTrigger kitchenTrigger;
    public CustomTrigger diningTrigger;


    void Awake()
    {
        kitchenTrigger.EnteredTrigger += OnKitchenTriggerEnter;
        diningTrigger.EnteredTrigger += OnDiningTriggerEnter;
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Player entered trigger - says " + collider.name);
    }
    void OnKitchenTriggerEnter(Collider collider)
    {
        Debug.Log("Player entered kitchen trigger - says " + collider.name);
    }

    void OnDiningTriggerEnter(Collider collider)
    {
        Debug.Log("Player entered dining trigger - says" + collider.name);
    }
}