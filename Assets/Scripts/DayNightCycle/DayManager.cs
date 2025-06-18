using System;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class DayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private TMP_Text currentDayText;
    public GameEvent onDayFinished;
    [Header("Variables")]
    [Range(0, 24)] public float timeOfDay;

    private float dayStartTime = 7;
    private const float TimeOfDaySpeed = 0.05f;
    public int currentDay;
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Cubemap skyboxA;
    [SerializeField] private Cubemap skyboxB;

    // Define customer active hours
    private const float CustomerSpawnStartTime = 9f; // 9 AM
    private const float CustomerSpawnEndTime = 21f; // 9 PM (21:00)

    private bool customersAreActive = false; // Track if customers should be spawning

    private void Start()
    {
        timeOfDay = dayStartTime;
        currentDay = 1; // Start at Day 1
        UpdateClock(timeOfDay); // Initial clock update
        UpdateCurrentDayText(); // Initial day text update
    }
    
    private void Update()
    {
        if (!preset)
            return;

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * TimeOfDaySpeed;
            
            // Check for new day
            if (timeOfDay >= 24f)
            {
                timeOfDay %= 24f;
                FinishDay(); // Advance to the next day
            }
            
            UpdateLighting(timeOfDay / 24f);
            UpdateClock(timeOfDay);
            HandleCustomerSpawningBasedOnTime();
        }
        else
        {
            UpdateLighting(timeOfDay / 24f);
            UpdateClock(timeOfDay);
        }
    }

    public void FinishDay()
    {
        currentDay++;
        timeOfDay = dayStartTime; // Reset time to start of day
        UpdateCurrentDayText();
        onDayFinished.Raise(this, currentDay);

        // Notify Customer Spawner to reset daily count for the new day
        if (CustomerSpawner.Instance != null)
        {
            CustomerSpawner.Instance.ResetDailyCustomers();
        }
        Debug.Log($"Day Finished! Moving to Day {currentDay}");
    }
    
    private void UpdateClock(float currentTime)
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
        clockText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    private void UpdateCurrentDayText()
    {
        currentDayText.text = "Day " + currentDay.ToString();
    }

    private void HandleCustomerSpawningBasedOnTime()
    {
        if (CustomerSpawner.Instance == null) return;

        // Check if current time is within active customer hours
        bool shouldBeActive = timeOfDay >= CustomerSpawnStartTime && timeOfDay < CustomerSpawnEndTime;

        if (shouldBeActive && !customersAreActive)
        {
            // Enter active hours
            customersAreActive = true;
            CustomerSpawner.Instance.EnableSpawning();
            Debug.Log($"Customer active hours started ({CustomerSpawnStartTime:00}:00).");
        }
        else if (!shouldBeActive && customersAreActive)
        {
            // Exit active hours
            customersAreActive = false;
            CustomerSpawner.Instance.DisableSpawning();
            Debug.Log($"Customer active hours ended ({CustomerSpawnEndTime:00}:00).");
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.ambientColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360) - 90, 170, 0));
        }
        
        float blend = Mathf.Clamp01(Mathf.Sin(timePercent * Mathf.PI));

        skyboxMaterial.SetTexture("_SkyboxA", skyboxA);
        skyboxMaterial.SetTexture("_SkyboxB", skyboxB);
        skyboxMaterial.SetFloat("_Blend", blend);

        RenderSettings.skybox = skyboxMaterial;

        DynamicGI.UpdateEnvironment();
    }
    
    private void OnValidate()
    {
        if (directionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}