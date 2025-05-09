using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace ANV
{
    public class AppearListInteractable : InteractableBase
    {
        private int boardOpenCount = 0; // Track the number of times the board is opened in this session
        public ListUI listUI; // Reference to the ListUI script
        public UGS_Analytics ugs_Analytics; // Reference to UGS_Analytics

        public override void OnInteract()
        {
            // Log for debugging purposes
            Debug.Log("AppearListInteractable: OnInteract called on " + gameObject.name);

            // Check if listUI is assigned
            if (listUI != null)
            {
                Debug.Log("AppearListInteractable: Toggling board UI.");
                listUI.ToggleBoardUI(); // Toggle the visibility of the board UI elements

                // Increment the counter each time the board is opened
                boardOpenCount++;

                //Debug.Log($"Board opened! Total opens this session: {boardOpenCount}");
            }
            else
            {
                Debug.LogError("AppearListInteractable: ListUI reference is not assigned.");
            }

            // Call the base OnInteract to retain any base functionality
            base.OnInteract();
        }

        private void OnApplicationQuit()
        {
            ugs_Analytics.BoardOpened(boardOpenCount);
        }

    }
}
