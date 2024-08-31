using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ListUI : MonoBehaviour
{
    public AppearObjects appearObjects; // Reference to the DataProvider script
    private float checkInterval = 0.5f; // Interval to check for initialization

    [SerializeField] private GameObject textPrefab; // Prefab for TextMeshProUGUI elements
    [SerializeField] private Transform contentPanel; // Content panel for the list items
    [SerializeField] private Canvas canvas; // Reference to the Canvas for UI elements
    private bool isInitialized = false; // Track whether initialization has been performed
    private bool isUIActive = false; // Track whether the UI is currently active

    public PlayerMovement playerMovement; // Reference to the PlayerMovement script
    public PlayerCamera playerCamera; // Reference to the PlayerCamera script

    void Start()
    {
        if (canvas == null)
        {
            Debug.LogError("Board Canvas is not assigned.");
            return;
        }
    }

    void Update()
    {
        if (isUIActive && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed");
            CloseBoardUI();
        }
    }

    void OnEnable()
    {
        // Start the coroutine when the GameObject becomes active
        if (!isInitialized)
        {
            StartCoroutine(WaitForInitialization());
            isInitialized = true;
        }
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

    public void ToggleBoardUI()
    {
        if (canvas != null)
        {
            bool isUIVisible = !canvas.gameObject.activeSelf;
            canvas.gameObject.SetActive(isUIVisible);

            if (isUIVisible)
            {
                PopulateList();
                isUIActive = true; // Set the UI active flag
                LockPlayerControls(); // Lock the player controls
            }
            else
            {
                UnlockPlayerControls(); // Unlock the player controls
            }
        }
        else
        {
            Debug.LogError("Board Canvas is not assigned.");
        }
    }

    public void CloseBoardUI()
    {
        isUIActive = false;
        canvas.gameObject.SetActive(false);
        UnlockPlayerControls();
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
                GameObject newItem = Instantiate(textPrefab, contentPanel);
                newItem.SetActive(true);

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

    private void LockPlayerControls()
    {
        // Lock the player's movement and camera controls
        FindObjectOfType<PlayerMovement>().enabled = false;
        FindObjectOfType<PlayerCamera>().enabled = false;

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UnlockPlayerControls()
    {
        // Unlock the player's movement and camera controls
        FindObjectOfType<PlayerMovement>().enabled = true;
        FindObjectOfType<PlayerCamera>().enabled = true;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
