using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class OwlScript : MonoBehaviour
{
    private Animator animator;
    
    public List<Transform> waypoints;
    public float speed = 5f;
    public float reachThreshold = 0.1f;

    public GameObject featherVFX;
    
    private int currentWaypointIndex = 0;
    public bool isFlying = false;
    private int direction = 1;

    [SerializeField] private AudioClip orderReadySFX;
    [SerializeField] private AudioClip owlFlyingSFX;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private StoreManager storeManager;
    
    // Bu flag ile başlangıç kodlarının sadece bir kere çalışmasını sağlayacağız
    private bool flyingStarted = false;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (isFlying)
        {
            FlyOwl();
        }
    }
    
    private void FlyOwl()
    {
        // Başlangıç kodları sadece bir kere çalışsın
        if (!flyingStarted)
        {
            animator.SetBool("isLanding", false);
            animator.SetBool("isFlying", true);
            gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            isFlying = true;
            Instantiate(featherVFX, transform.position, Quaternion.identity);
            
            audioSource.clip = owlFlyingSFX;
            audioSource.loop = true;
            audioSource.Play();
            ToggleAllWindowsSimple();
            
            flyingStarted = true; // Flag'i true yap ki bir daha çalışmasın
        }
        
        if (waypoints == null || waypoints.Count == 0)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 directionToTarget = (targetWaypoint.position - transform.position).normalized;
        transform.position += directionToTarget * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, targetWaypoint.position) < reachThreshold)
        {
            currentWaypointIndex += direction;

            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = waypoints.Count - 2;
                direction = -1;
                gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else if (currentWaypointIndex < 0)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
                animator.SetBool("isLanding", true);
                animator.SetBool("isFlying", false);
                isFlying = false;
                flyingStarted = false; // Uçuş bittiğinde flag'i resetle
                storeManager.ProcessPendingIngredients();
                audioSource.loop = false;
                audioSource.clip = orderReadySFX;
                audioSource.Play();
                currentWaypointIndex = 1;
                direction = 1;
                ToggleAllWindowsSimple();
            }
        }
    }
    
    private void ToggleAllWindowsSimple()
    {
        WindowScript[] allWindowScripts = FindObjectsByType<WindowScript>(0);
        
        foreach (WindowScript windowScript in allWindowScripts)
        {
            if (windowScript != null)
            {
                windowScript.ToggleWindow();
            }
        }
    }
}