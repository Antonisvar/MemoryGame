using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    float elapsedTime = 0f;
    string filePath;

    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/elapsedTime.txt";
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnApplicationQuit()
    {
        SaveElapsedTime();  // Save elapsed time when the application quits
    }

    void SaveElapsedTime()
    {
        // Convert elapsedTime to string and append it to the file
        int seconds = Mathf.FloorToInt(elapsedTime);
        string timeString = seconds.ToString();  // Format with 2 decimal places
        File.AppendAllText(filePath, timeString + Environment.NewLine);  // Append time to file with newline
    }
}
