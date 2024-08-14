using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerTrigger : MonoBehaviour
{
    // Triggers for every room
    public CustomTrigger kitchenTrigger;
    public CustomTrigger diningTrigger;
    public CustomTrigger TvRoomTrigger;
    public CustomTrigger Bathroom1Trigger;
    public CustomTrigger Bathroom2Trigger;
    public CustomTrigger Bedroom1Trigger;
    public CustomTrigger Bedroom2Trigger;
    public CustomTrigger Bedroom3Trigger;

    // Counters for every room
    private int kitchenCount = 0;
    private int diningCount = 0;
    private int TvRoomCount = 0;
    private int Bathroom1Count = 0;
    private int Bathroom2Count = 0;
    private int Bedroom1Count = 0;
    private int Bedroom2Count = 0;
    private int Bedroom3Count = 0;

    // File paths for saving the counters
    private string kitchenCountFilePath;
    private string diningCountFilePath;
    private string TvRoomCountFilePath;
    private string Bathroom1CountFilePath;
    private string Bathroom2CountFilePath;
    private string Bedroom1CountFilePath;
    private string Bedroom2CountFilePath;
    private string Bedroom3CountFilePath;

    void Awake()
    {
        // Subscribe to the EnteredTrigger event for every room
        kitchenTrigger.EnteredTrigger += OnKitchenTriggerEnter;
        diningTrigger.EnteredTrigger += OnDiningTriggerEnter;
        TvRoomTrigger.EnteredTrigger += OnTvRoomTriggerEnter;
        Bathroom1Trigger.EnteredTrigger += OnBathroom1TriggerEnter;
        Bathroom2Trigger.EnteredTrigger += OnBathroom2TriggerEnter;
        Bedroom1Trigger.EnteredTrigger += OnBedroom1TriggerEnter;
        Bedroom2Trigger.EnteredTrigger += OnBedroom2TriggerEnter;
        Bedroom3Trigger.EnteredTrigger += OnBedroom3TriggerEnter;

        // Set the file paths for saving the counters
        kitchenCountFilePath = Path.Combine(Application.persistentDataPath, "kitchenCount.txt");
        diningCountFilePath = Path.Combine(Application.persistentDataPath, "diningCount.txt");
        TvRoomCountFilePath = Path.Combine(Application.persistentDataPath, "TvRoomCount.txt");
        Bathroom1CountFilePath = Path.Combine(Application.persistentDataPath, "Bathroom1Count.txt");
        Bathroom2CountFilePath = Path.Combine(Application.persistentDataPath, "Bathroom2Count.txt");
        Bedroom1CountFilePath = Path.Combine(Application.persistentDataPath, "Bedroom1Count.txt");
        Bedroom2CountFilePath = Path.Combine(Application.persistentDataPath, "Bedroom2Count.txt");
        Bedroom3CountFilePath = Path.Combine(Application.persistentDataPath, "Bedroom3Count.txt");

    }

    // Methods for handling the EnteredTrigger event for every room
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

    void OnTvRoomTriggerEnter(Collider collider)
    {
        Debug.Log("Player entered TvRoom trigger - says" + collider.name);
        TvRoomCount++;
    }

    void OnBathroom1TriggerEnter(Collider collider)
    {
        Debug.Log("Player entered Bathroom1 trigger - says" + collider.name);
        Bathroom1Count++;
    }

    void OnBathroom2TriggerEnter(Collider collider)
    {
        Debug.Log("Player entered Bathroom2 trigger - says" + collider.name);
        Bathroom2Count++;
    }

    void OnBedroom1TriggerEnter(Collider collider)
    {
        Debug.Log("Player entered Bedroom1 trigger - says" + collider.name);
        Bedroom1Count++;
    }

    void OnBedroom2TriggerEnter(Collider collider)
    {
        Debug.Log("Player entered Bedroom2 trigger - says" + collider.name);
        Bedroom2Count++;
    }

    void OnBedroom3TriggerEnter(Collider collider)
    {
        Debug.Log("Player entered Bedroom3 trigger - says" + collider.name);
        Bedroom3Count++;
    }

    // Method for saving the counter to a file
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
        // Save the counters to files when the application is closed
        SaveCounter(kitchenCountFilePath, kitchenCount);
        SaveCounter(diningCountFilePath, diningCount);
        SaveCounter(TvRoomCountFilePath, TvRoomCount);
        SaveCounter(Bathroom1CountFilePath, Bathroom1Count);
        SaveCounter(Bathroom2CountFilePath, Bathroom2Count);
        SaveCounter(Bedroom1CountFilePath, Bedroom1Count);
        SaveCounter(Bedroom2CountFilePath, Bedroom2Count);
        SaveCounter(Bedroom3CountFilePath, Bedroom3Count);

        Debug.Log("file saved on " + Application.persistentDataPath);
    }
}