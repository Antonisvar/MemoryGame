using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TriggerCounter : MonoBehaviour
{
    private int counter = 0;
    private string filePath;

    // Initialize with default folder
    void Start()
    {
        SetFilePath("default_folder");
    }

    void OnTriggerEnter(Collider other)
    {
        counter++;
        Debug.Log("Triggered " + counter + " times");
    }

    void OnApplicationQuit()
    {
        SaveCounterToFile();
    }

    public void SetFilePath(string folderName)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        filePath = Path.Combine(directoryPath, "trigger_count.txt");
    }

    public void SaveCounterToFile()
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(counter.ToString());
        }
        Debug.Log("Counter value appended to " + filePath);
    }
}
