using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private int counter = 0;

    void OnTriggerEnter(Collider other)
    {
        counter++;
        Debug.Log("Triggered " + counter + " times");
    }

    void OnApplicationQuit()
    {
        SaveCounterToFile();
    }

    void SaveCounterToFile()
    {
        string path = "D:/recordings/diploma/counter.txt";
        //Append the current counter value to the file
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(counter);
        }
        Debug.Log("Counter saved to " + path);
    }
}
