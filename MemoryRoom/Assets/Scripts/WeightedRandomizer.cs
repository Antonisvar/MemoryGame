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

    // Public property to access instantiatedObjects
    public List<GameObject> InstantiatedObjects => instantiatedObjects;

    void Start()
    {
        instantiatedObjects = new List<GameObject>(); // Initialize instantiated objects list

        // Load all item prefabs from the "Targets" folder inside the Resources directory
        itemPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Targets"));
        //Debug.Log($"Loaded {itemPrefabs.Count} item prefabs from Targets folder.");

        // Adjust maxItems based on time
        AdjustMaxItemsBasedOnTime(Application.persistentDataPath + "/elapsedTime.txt");

        // Load and invert the weights
        string[] roomFiles = new string[]
        {
            Path.Combine(Application.persistentDataPath, "kitchenCount.txt"),
            Path.Combine(Application.persistentDataPath, "diningCount.txt"),
            Path.Combine(Application.persistentDataPath, "TvRoomCount.txt"),
            Path.Combine(Application.persistentDataPath, "Bathroom1Count.txt"),
            Path.Combine(Application.persistentDataPath, "Bathroom2Count.txt"),
            Path.Combine(Application.persistentDataPath, "Bedroom1Count.txt"),
            Path.Combine(Application.persistentDataPath, "Bedroom2Count.txt"),
            Path.Combine(Application.persistentDataPath, "Bedroom3Count.txt")
        };
        LoadWeightsFromFile(roomFiles);

        // Find trigger zones and load their positions
        LoadPositionsFromTriggerZones();

        // Calculate how many items to spawn in each room
        Dictionary<string, int> itemsToSpawn = CalculateItemsToSpawn();
        //Debug.Log("Calculated Items to Spawn: " + string.Join(", ", itemsToSpawn.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
        int totalItemsInstantiated = 0;

        // Spawn items in each room, respecting total limit
        foreach (var room in roomTriggerZones)
        {
            string roomName = room.name.Replace("Trigger", "").Replace("Count", "").ToLower();
            if (itemsToSpawn.ContainsKey(roomName) && roomPositions.ContainsKey(roomName))
            {
                int itemCount = itemsToSpawn[roomName];
                List<Vector3> availablePositions = roomPositions[roomName];

                // Log itemCount and availablePositions count
                Debug.Log($"Room: {roomName}, Item Count: {itemCount}, Available Positions: {availablePositions.Count}");


                // Ensure we don't exceed the number of available positions for that room
                itemCount = Mathf.Min(itemCount, availablePositions.Count);

                if (itemCount > 0)
                {
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
                        //Debug.Log($"Available Positions for {roomName}: {roomPositions[roomName].Count}");

                        //Debug.Log($"Instantiating {itemCount} items in {roomName}");

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


    void LoadWeightsFromFile(string[] filePaths)
    {
        // Clear the dictionary before loading new weights
        roomWeights.Clear();

        foreach (var filePath in filePaths)
        {
            string path = Path.Combine(Application.persistentDataPath, filePath);

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                if (lines.Length > 0)
                {
                    int visitCount = 0;


                    // Calculate the average visit count across all rounds
                    int totalVisits = 0;
                    foreach (string line in lines)
                    {
                        if (int.TryParse(line, out int count))
                        {
                            totalVisits += count;
                        }
                    }
                    visitCount = Mathf.Max(totalVisits / lines.Length, 1); // Prevent zero average

                    // Assign the room name based on the file name
                    string roomName = Path.GetFileNameWithoutExtension(filePath);

                    // Invert the weight based on visit count (fewer visits = higher weight)
                    float invertedWeight = 1f / visitCount;

                    // Store the inverted weight for the room
                    roomWeights[roomName] = invertedWeight;

                    // Debug log to check values
                    Debug.Log($"Room: {roomName}, Visit Count: {visitCount}, Inverted Weight: {invertedWeight}");

                }
                else
                {
                    Debug.LogWarning("Visit count file is empty: " + path);
                }
            }
            else
            {
                Debug.LogError("Visit count file not found: " + path);
            }
        }
    }


    void LoadPositionsFromTriggerZones()
    {
        roomPositions.Clear(); // Clear any existing data

        int placeableLayer = LayerMask.GetMask("Placeable"); // Get the LayerMask for the "Placeable" layer
        float minDistanceBetweenItems = 1.5f; // Minimum distance between items to avoid overlap
        int maxAttempts = 1000; // Maximum attempts to find a valid position
        float raycastDistance = 20f; // Distance for raycast to ensure it hits the correct surface

        foreach (var triggerZone in roomTriggerZones)
        {
            Collider collider = triggerZone.GetComponent<Collider>();
            if (collider != null && collider.isTrigger)
            {
                string roomName = triggerZone.name.Replace("Trigger", "").Replace("Count", "").ToLower();
                Bounds bounds = collider.bounds;

                List<Vector3> positions = new List<Vector3>(); // List to store unique positions

                int numberOfPositions = 10; // Number of positions to generate per room

                for (int i = 0; i < numberOfPositions; i++)
                {
                    bool positionFound = false;
                    int attempts = 0;

                    while (!positionFound && attempts < maxAttempts)
                    {
                        // Generate a random position within the bounds of the room
                        Vector3 randomPosition = new Vector3(
                            Random.Range(bounds.min.x, bounds.max.x),
                            bounds.max.y, // Start the raycast from the top of the collider bounds
                            Random.Range(bounds.min.z, bounds.max.z)
                        );

                        // Raycast downwards from a height above the collider bounds to find a surface on the "Placeable" layer
                        RaycastHit hit;
                        Vector3 rayStartPosition = new Vector3(randomPosition.x, bounds.max.y + 1f, randomPosition.z);

                        Debug.DrawRay(rayStartPosition, Vector3.down * raycastDistance, Color.green, 1f); // Debug line for visualization

                        if (Physics.Raycast(rayStartPosition, Vector3.down, out hit, raycastDistance, placeableLayer))
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
                                // If the position is valid and not too close to other items, add it
                                positions.Add(newPosition);
                                //Debug.Log($"Position found for room {roomName}: {newPosition}");
                                positionFound = true;
                            }
                        }
                        attempts++;
                    }

                    if (attempts >= maxAttempts)
                    {
                        Debug.LogWarning($"Could not find a valid position after max attempts in {roomName}");
                    }
                }

                roomPositions[roomName] = positions; // Store the generated unique positions for this room
                Debug.Log($"Positions generated for room {roomName}: {positions.Count}");
            }
            else
            {
                Debug.LogWarning($"Room Trigger Zone does not have a valid trigger collider: {triggerZone.name}");
            }
        }
    }

    Dictionary<string, int> CalculateItemsToSpawn()
    {
        // Ensure maxItems is greater than 0
        if (maxItems <= 0)
        {
            Debug.LogError("Max items is less than or equal to zero!");
            return new Dictionary<string, int>();
        }

        // Ensure we have weights to calculate from
        if (roomWeights.Count == 0)
        {
            Debug.LogError("No room weights loaded!");
            return new Dictionary<string, int>();
        }

        // Calculate the total inverted weight
        float totalInvertedWeight = roomWeights.Values.Sum();

        // Ensure totalInvertedWeight is not zero
        if (totalInvertedWeight == 0)
        {
            Debug.LogError("Total inverted weight is zero!");
            return new Dictionary<string, int>();
        }

        Dictionary<string, int> itemsToSpawn = new Dictionary<string, int>();

        foreach (var room in roomWeights)
        {
            string standardizedRoomName = room.Key.Replace("Trigger", "").Replace("Count", "").ToLower();

            // Calculate how many items to spawn in this room based on the room's weight
            float weightPercentage = room.Value / totalInvertedWeight;
            int itemCount = Mathf.RoundToInt(weightPercentage * maxItems);

            itemsToSpawn[standardizedRoomName] = itemCount;

            // Debug output to check calculations
            Debug.Log($"Room for Item To Spawn: {room.Key}, Weight: {room.Value}, Items to Spawn: {itemCount}");

        }

        return itemsToSpawn;
    }
}
