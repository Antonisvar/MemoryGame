using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerTrigger : MonoBehaviour
{
    public CustomTrigger kitchenTrigger;
    public CustomTrigger diningTrigger;

    private int kitchenCount = 0;
    private int diningCount = 0;
    private string kitchenCountFilePath;
    private string diningCountFilePath;
    void Awake()
    {
        kitchenTrigger.EnteredTrigger += OnKitchenTriggerEnter;
        diningTrigger.EnteredTrigger += OnDiningTriggerEnter;

        kitchenCountFilePath = Path.Combine(Application.persistentDataPath, "kitchenCount.txt");
        diningCountFilePath = Path.Combine(Application.persistentDataPath, "diningCount.txt");

    }

    void OnKitchenTriggerEnter(Collider collider)
    {
        Debug.Log("Player entered kitchen trigger - says " + collider.name);
        kitchenCount++;
    }

    void OnDiningTriggerEnter(Collider collider)
    {
        Debug.Log("Player entered dining trigger - says" + collider.name);
        diningCount++;
    }

    void SaveCounter(string filePath, int count)
    {
        try
        {
            File.AppendAllText(filePath, count.ToString() + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save counter to file: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        SaveCounter(kitchenCountFilePath, kitchenCount);
        SaveCounter(diningCountFilePath, diningCount);
    }
}