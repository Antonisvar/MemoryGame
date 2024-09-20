using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using ANV; // Ensure this namespace is used for the InteractableBase and InteractionController

public class WeightedRandomizer : MonoBehaviour
{
    private Dictionary<string, float> roomWeights = new Dictionary<string, float>();
    private Dictionary<string, List<Vector3>> roomPositions = new Dictionary<string, List<Vector3>>();

    public GameObject[] roomTriggerZones; // Assign in the Unity Editor
    public int baseMaxItems = 5; // Base number of items
    private int maxItems; // Dynamically adjusted based on time
    public float timeThreshold = 6.0f; // Time threshold in minutes

    private List<GameObject> instantiatedObjects; // List to track instantiated objects
    private List<GameObject> itemPrefabs; // List to store loaded item prefabs
    private InteractionController interactionController; // Reference to the InteractionController

    void Start()
    {
        instantiatedObjects = new List<GameObject>(); // Initialize instantiated objects list

        // Load all item prefabs from the "Targets" folder inside the Resources directory
        itemPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Targets"));

        // Adjust maxItems based on time
        AdjustMaxItemsBasedOnTime(Application.persistentDataPath + "/elapsedTime.txt");

        // Load and invert the weights
        LoadWeightsFromFile("weights.txt");

        // Find trigger zones and load their positions
        LoadPositionsFromTriggerZones();

        // Calculate how many items to spawn in each room
        Dictionary<string, int> itemsToSpawn = CalculateItemsToSpawn();

        int totalItemsInstantiated = 0;

        // Spawn items in each room, respecting total limit
        foreach (var room in roomTriggerZones)
        {
            string roomName = room.name;
            if (itemsToSpawn.ContainsKey(roomName) && roomPositions.ContainsKey(roomName))
            {
                int itemCount = itemsToSpawn[roomName];
                List<Vector3> availablePositions = roomPositions[roomName];

                // Ensure we don't exceed the number of available positions for that room
                itemCount = Mathf.Min(itemCount, availablePositions.Count);

                for (int i = 0; i < itemCount; i++)
                {
                    if (totalItemsInstantiated >= maxItems)
                    {
                        Debug.Log("Item limit reached. Stopping instantiation.");
                        return;
                    }

                    // Get a random position from the available positions
                    int randomIndex = Random.Range(0, availablePositions.Count);
                    Vector3 position = availablePositions[randomIndex];

                    // Select a random item prefab from the loaded itemPrefabs list
                    GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

                    // Instantiate the random item prefab at the specific position and track it
                    GameObject instantiatedObject = Instantiate(randomPrefab, position, Quaternion.identity);

                    // Add instantiated objects to the tracking list only if they are interactable
                    if (instantiatedObject.layer == LayerMask.NameToLayer("Interactable"))
                    {
                        instantiatedObjects.Add(instantiatedObject);
                    }

                    totalItemsInstantiated++;

                    // Remove the used position to ensure it's not reused
                    availablePositions.RemoveAt(randomIndex);
                }
            }
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

    void AdjustMaxItemsBasedOnTime(string filePath)
    {
        string path = Path.Combine(Application.dataPath, filePath);

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            if (lines.Length > 0)
            {
                // Get the last recorded time (most recent)
                string lastTimeString = lines[lines.Length - 1];

                // Parse the time from the string
                if (float.TryParse(lastTimeString, out float timeElapsed))
                {
                    // Adjust maxItems based on the elapsed time
                    if (timeElapsed < timeThreshold)
                    {
                        maxItems = baseMaxItems + 1; // Add one item if time is below the threshold
                    }
                    else
                    {
                        maxItems = baseMaxItems - 1; // Remove one item if time is above the threshold
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse the time from the timer file.");
                    maxItems = baseMaxItems; // Default to baseMaxItems if parsing fails
                }
            }
            else
            {
                Debug.LogError("Timer file is empty.");
                maxItems = baseMaxItems; // Default to baseMaxItems if the file is empty
            }
        }
        else
        {
            Debug.LogError("Timer file not found: " + path);
            maxItems = baseMaxItems; // Default to baseMaxItems if the file is not found
        }
    }


    void LoadWeightsFromFile(string filePath)
    {
        string path = Path.Combine(Application.dataPath, filePath);
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                string[] parts = line.Split(' ');
                if (parts.Length == 2)
                {
                    string roomName = parts[0];
                    float weight = float.Parse(parts[1]);
                    roomWeights[roomName] = 1 / weight; // Invert the weight
                }
            }
        }
        else
        {
            Debug.LogError("Weights file not found: " + path);
        }
    }

    void LoadPositionsFromTriggerZones()
    {
        roomPositions.Clear(); // Clear any existing data

        int placeableLayer = LayerMask.GetMask("Placeable"); // Get the LayerMask for the "Placeable" layer
        float minDistanceBetweenItems = 1.5f; // Minimum distance between items to avoid overlap
        int maxAttempts = 100; // Maximum attempts to find a valid position

        foreach (var triggerZone in roomTriggerZones)
        {
            Collider collider = triggerZone.GetComponent<Collider>();
            if (collider != null && collider.isTrigger)
            {
                string roomName = triggerZone.name;
                Vector3 triggerCenter = collider.bounds.center;
                Vector3 triggerSize = collider.bounds.size;

                List<Vector3> positions = new List<Vector3>(); // List to store unique positions
                float raycastDistance = 10f; // Example raycast distance

                int numberOfPositions = 10; // Example number of positions

                for (int i = 0; i < numberOfPositions; i++)
                {
                    bool positionFound = false;
                    int attempts = 0;

                    while (!positionFound && attempts < maxAttempts)
                    {
                        // Generate a random position within the trigger zone bounds
                        Vector3 randomPosition = new Vector3(
                            Random.Range(-triggerSize.x / 2, triggerSize.x / 2),
                            triggerSize.y / 2, // Start the raycast from the top of the room
                            Random.Range(-triggerSize.z / 2, triggerSize.z / 2)
                        ) + triggerCenter;

                        // Perform a downward raycast to find a valid surface on the "Placeable" layer
                        RaycastHit hit;
                        if (Physics.Raycast(randomPosition, Vector3.down, out hit, raycastDistance, placeableLayer))
                        {
                            Vector3 newPosition = hit.point;

                            // Check if the new position is far enough from all existing positions
                            bool isTooClose = false;
                            foreach (Vector3 existingPosition in positions)
                            {
                                if (Vector3.Distance(newPosition, existingPosition) < minDistanceBetweenItems)
                                {
                                    isTooClose = true;
                                    break;
                                }
                            }

                            if (!isTooClose)
                            {
                                // If the position is valid and not too close to existing ones, add it
                                positions.Add(newPosition);
                                positionFound = true;
                            }
                        }

                        attempts++;
                    }

                    if (attempts >= maxAttempts)
                    {
                        Debug.LogWarning("Could not find a valid position after max attempts in " + roomName);
                    }
                }

                roomPositions[roomName] = positions; // Store generated unique positions for this room
            }
        }
    }

    Dictionary<string, int> CalculateItemsToSpawn()
    {
        float totalInvertedWeight = roomWeights.Values.Sum();

        Dictionary<string, int> itemsToSpawn = new Dictionary<string, int>();
        foreach (var room in roomWeights)
        {
            int itemCount = Mathf.RoundToInt((room.Value / totalInvertedWeight) * maxItems);
            itemsToSpawn[room.Key] = itemCount;
        }

        return itemsToSpawn;
    }
}
