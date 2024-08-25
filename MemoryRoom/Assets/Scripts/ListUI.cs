using System.Collections;
using UnityEngine;
using TMPro;

public class ListUI : MonoBehaviour
{
    public AppearObjects appearObjects; // Reference to the DataProvider script
    private float checkInterval = 0.5f; // Interval to check for initialization

    // Prefab or component used to create new TextMeshProUGUI elements
    [SerializeField] private GameObject textPrefab;

    private Canvas canvas; // Reference to the Canvas to hold the Text elements
    [SerializeField] private Transform contentPanel; // Reference to the content panel that holds the UI elements

    void Start()
    {
        // Find the Canvas in the scene
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene.");
            return;
        }

        StartCoroutine(WaitForInitialization());
    }

    IEnumerator WaitForInitialization()
    {
        // Wait until appearObjects is assigned and the list is initialized
        while (appearObjects == null || appearObjects.InstantiatedObjects == null)
        {
            yield return new WaitForSeconds(checkInterval);
        }

        // Wait until the list is populated
        while (appearObjects.InstantiatedObjects.Count == 0)
        {
            yield return new WaitForSeconds(checkInterval);
        }

        PopulateList();
    }

    void PopulateList()
    {
        // Clear existing UI elements if needed
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Create a new TextMeshProUGUI element for each item
        foreach (var item in appearObjects.InstantiatedObjects)
        {
            if (item != null)
            {
                // Create a new GameObject and add a TextMeshProUGUI component
                GameObject newItem = Instantiate(textPrefab, contentPanel);

                // Ensure the new item is active
                newItem.SetActive(true);

                // Set the text of the new item to the name of the item
                TextMeshProUGUI itemText = newItem.GetComponent<TextMeshProUGUI>();

                if (itemText != null)
                {
                    itemText.text = item.name;
                }
                else
                {
                    Debug.LogError("textPrefab does not have a TextMeshProUGUI component.");
                }
            }
            else
            {
                Debug.LogWarning("Null entry found in InstantiatedObjects list.");
            }
        }
    }
}
