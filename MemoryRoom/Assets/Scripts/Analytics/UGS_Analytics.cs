using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Analytics;


public class UGS_Analytics : MonoBehaviour
{
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            GiveConsent(); //Get user consent according to various legislations
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void GiveConsent()
    {
        // Call if consent has been given by the user
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log($"Consent has been provided. The SDK is now collecting data!");
    }

    public void BoardOpened(int counter)
    {
        CustomEvent boardOpened = new CustomEvent("board_opened")
        {
                {"session_open_count", counter},
        };
        AnalyticsService.Instance.RecordEvent(boardOpened);

        Debug.Log("Recording event Board opened");
    }

    public void TimeTaken(int time)
    {
        CustomEvent time_taken = new CustomEvent("time_taken")
        {
                {"time_per_session", time},
        };
        AnalyticsService.Instance.RecordEvent(time_taken);

        Debug.Log("Recording event Time Taken");
    }

    public void NotValidE(int counter)
    {
        CustomEvent notValidE = new CustomEvent("missclickE")
        {
                {"not_valid_E", counter},
        };
        AnalyticsService.Instance.RecordEvent(notValidE);

        Debug.Log("Recording event missclick E");
    }

    // Call this when the user finds an item
    public void LogItemFound(string itemId, int timeTaken)
    {
        CustomEvent item_found_avg = new CustomEvent("item_found_avg")
        {
            { "item_id", itemId },    // Unique identifier for the item
            { "time_taken", timeTaken } // Time it took to find this item
        };

        AnalyticsService.Instance.RecordEvent(item_found_avg);

        Debug.Log($"Logged item_found event: Item {itemId}, Time taken: {timeTaken}s");
    }

}