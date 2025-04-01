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
    [Header("Variables")]
    [Range(0, 24)] public float timeOfDay;
    private const float TimeOfDaySpeed = 0.01675f;
    public int currentDay;
    

    private void Update()
    {
        if (!preset)
            return;

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * TimeOfDaySpeed;
            timeOfDay %= 24f;
            UpdateLighting(timeOfDay / 24f);
            UpdateClock(timeOfDay);
        }
        else
        {
            UpdateLighting(timeOfDay / 24f);
        }
    }

    private void UpdateClock(float currentTime)
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
        clockText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        currentDayText.text = currentDay.ToString();
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
