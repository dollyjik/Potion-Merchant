using System;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    
    [Header("Day Manager References")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private int plantDay;
    [SerializeField] private float plantTime;
    [SerializeField] private AudioClip plantSound;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private PlantStateMachine stateMachine;
    
    private bool isSoundPlayed;
    
    [SerializeField] private JarScript[] jars;

    private void Start()
    {
        GameObject audioGameObject = GameObject.FindGameObjectWithTag("owlInteraction");
        audioSource = audioGameObject.GetComponent<AudioSource>();
        jars = FindObjectsByType<JarScript>(0);

        stateMachine = GetComponent<PlantStateMachine>();
        
        dayManager = FindAnyObjectByType<DayManager>();
        plantDay = dayManager.currentDay;
        plantTime = dayManager.timeOfDay;
    }

    private void Update()
    {
        if (stateMachine.currentState == stateMachine.fruitState && !isSoundPlayed)
        {
            audioSource.PlayOneShot(plantSound, .1f);
            isSoundPlayed = true;
        }

        if (stateMachine.currentState != stateMachine.fruitState )
        {
            isSoundPlayed = false;
        }
    }
}
