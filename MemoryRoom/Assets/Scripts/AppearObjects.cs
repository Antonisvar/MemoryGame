using System.Collections.Generic;
using UnityEngine;
using ANV; // Ensure this namespace is used for the InteractableBase and InteractionController

public class AppearObjects : MonoBehaviour
{
    public List<GameObject> objectsToAppear; // List of objects to instantiate
    private List<GameObject> instantiatedObjects; // List to track instantiated objects
    private InteractionController interactionController; // Reference to the InteractionController

    void Start()
    {
        // Load objects and initialize lists
        objectsToAppear = new List<GameObject>(Resources.LoadAll<GameObject>("Targets"));
        instantiatedObjects = new List<GameObject>();

        // Instantiate objects and add them to the list if they are interactable
        float i = 0.5f;
        foreach (var obj in objectsToAppear)
        {
            GameObject instantiatedObject = Instantiate(obj, new Vector3(0.5f + i, 1f, -5.97f), Quaternion.identity);
            if (instantiatedObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                instantiatedObjects.Add(instantiatedObject);
            }
            i += 0.5f;
        }

        // Get the InteractionController from the scene
        interactionController = FindObjectOfType<InteractionController>();
        if (interactionController == null)
        {
            Debug.LogError("No InteractionController found in the scene.");
        }
    }

    void Update()
    {
        if (interactionController == null)
            return;

        // Get the current interactable object from the InteractionController
        InteractableBase interactable = interactionController.interactionData.Interactable;

        if (interactable != null)
        {
            // Check if the interactable object is in the instantiatedObjects list
            if (instantiatedObjects.Contains(interactable.gameObject))
            {
                // The object is interactable, so we check for interactions
                if (interactionController.interactionInputData.InteractedClicked)
                {
                    // Call the OnInteract method of the interactable object
                    interactable.OnInteract();
                    // Remove the object from the list and destroy it
                    instantiatedObjects.Remove(interactable.gameObject);
                    Destroy(interactable.gameObject);

                    // Check if the list is empty and close the game if it is
                    if (instantiatedObjects.Count == 0)
                    {
                        // Optionally save game state or perform other cleanup tasks here

                        // Close the game
                        Application.Quit();
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#endif
                    }
                }
            }
        }
    }
}
