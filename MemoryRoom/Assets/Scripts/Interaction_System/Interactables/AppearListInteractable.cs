using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    public class AppearListInteractable : InteractableBase
    {
        public ListUI listUI; // Reference to the ListUI script

        public override void OnInteract()
        {
            // Log for debugging purposes
            Debug.Log("AppearListInteractable: OnInteract called on " + gameObject.name);

            // Check if listUI is assigned
            if (listUI != null)
            {
                Debug.Log("AppearListInteractable: Toggling board UI.");
                listUI.ToggleBoardUI(); // Toggle the visibility of the board UI elements
            }
            else
            {
                Debug.LogError("AppearListInteractable: ListUI reference is not assigned.");
            }

            // Call the base OnInteract to retain any base functionality
            base.OnInteract();
        }
    }
}
