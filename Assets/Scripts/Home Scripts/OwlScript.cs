using System;
using UnityEngine;
using System.Collections.Generic;

public class OwlScript : MonoBehaviour
{
    private Animator animator;
    
    public List<Transform> waypoints;
    public float speed = 5f;
    public float reachThreshold = 0.1f; 

    private int currentWaypointIndex = 0;
    private bool isFlying = false;
    private int direction = 1; 

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
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("isFlying", true);
            gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            isFlying = true;
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            animator.SetBool("isLanding", true);
            animator.SetBool("isFlying", false);
            gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
            isFlying = false;
        }
    }

    private void FlyOwl()
    {
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
                animator.SetBool("isLanding", true);
                animator.SetBool("isFlying", false);
                isFlying = false;
                currentWaypointIndex = 1;
                direction = 1;
            }
        }
    }
}