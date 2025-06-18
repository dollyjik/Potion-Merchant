using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance; // Add a static instance for easy access

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] customerPrefabs; // Assign all your Customer Prefabs here
    [SerializeField] private Transform spawnPoint; // Point where customers will spawn
    [SerializeField] private float spawnInterval = 10f; // Time between potential spawns
    [SerializeField] private int maxDailyCustomers = 20; // Maximum customers to spawn in a "day"
    [SerializeField] private int queueFullThreshold = 3; // YENİ EKLENDİ: Sıra doluluk eşiği

    private int customersSpawnedToday = 0;
    private bool canSpawnCustomers = false; // Changed initial state to false

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("Customer Prefabs array is empty or not assigned in CustomerSpawner. Please assign them in the Inspector.");
            enabled = false; // Disable the script if no prefabs are assigned
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn Point is not assigned in CustomerSpawner. Using spawner's position as default.");
            spawnPoint = this.transform;
        }

        // We will start the coroutine when spawning is enabled by DayManager
    }

    // New method to enable spawning
    public void EnableSpawning()
    {
        if (!canSpawnCustomers)
        {
            canSpawnCustomers = true;
            Debug.Log("Customer spawning enabled.");
            StartCoroutine(SpawnCustomersRoutine());
        }
    }

    // New method to disable spawning
    public void DisableSpawning()
    {
        if (canSpawnCustomers)
        {
            canSpawnCustomers = false;
            Debug.Log("Customer spawning disabled.");
            StopAllCoroutines(); // Stop any active spawning routine
        }
    }

    private IEnumerator SpawnCustomersRoutine()
    {
        while (canSpawnCustomers && customersSpawnedToday < maxDailyCustomers) // Check canSpawnCustomers here too
        {
            // YENİ KONTROL: Sıra doluluk kontrolü
            if (CustomerQueueManager.Instance != null && 
                CustomerQueueManager.Instance.CurrentQueueCount >= (CustomerQueueManager.Instance.MaxQueueCapacity - queueFullThreshold))
            {
                Debug.Log($"Queue is almost full ({CustomerQueueManager.Instance.CurrentQueueCount}/{CustomerQueueManager.Instance.MaxQueueCapacity}). Waiting to spawn...");
                yield return new WaitForSeconds(1f); // Kısa bir süre bekle ve tekrar kontrol et
                continue; // Döngüyü başa sar, spawn etme
            }

            SpawnCustomer();
            customersSpawnedToday++;
            yield return new WaitForSeconds(spawnInterval);
        }

        if (customersSpawnedToday >= maxDailyCustomers)
        {
            Debug.Log("Daily customer limit reached. Spawning stopped for now.");
            DisableSpawning(); // Automatically disable spawning once limit is hit
        }
    }

    private void SpawnCustomer()
    {
        // Randomly select a customer prefab from the array
        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject selectedCustomerPrefab = customerPrefabs[randomIndex];

        Instantiate(selectedCustomerPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"Customer ({selectedCustomerPrefab.name}) spawned. Total customers today: {customersSpawnedToday + 1}");
    }

    // Call this method to reset the daily customer count, e.g., at the start of a new in-game day
    public void ResetDailyCustomers()
    {
        customersSpawnedToday = 0;
        // Spawning will be re-enabled by DayManager at the appropriate time
        Debug.Log("Daily customer count reset.");
    }
}