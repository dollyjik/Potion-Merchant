using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerQueueManager : MonoBehaviour
{
    public static CustomerQueueManager Instance;

    [SerializeField] private Transform[] queuePositions;
    [SerializeField] private Transform orderPoint;
    [SerializeField] private Transform exitPoint;

    [SerializeField] private AudioClip waitSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool isSoundPlayed;
    
    public Queue<Customer> customerQueue = new Queue<Customer>();
    public Customer currentCustomer;

    public Transform OrderPoint => orderPoint;
    public Transform ExitPoint => exitPoint;
    
    public int CurrentQueueCount => customerQueue.Count;
    public int MaxQueueCapacity => queuePositions.Length;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void JoinQueue(Customer customer)
    {
        customerQueue.Enqueue(customer);
        UpdateQueuePositions();
    }

    private void Update()
    {
        if (currentCustomer == null && customerQueue.Count > 0)
        {
            currentCustomer = customerQueue.Dequeue();
            currentCustomer.SetTarget(orderPoint);
        }

        if (customerQueue.Count > 3 && !isSoundPlayed)
        {
            audioSource.PlayOneShot(waitSound);
            isSoundPlayed = true;
        }
    }

    public void NotifyCustomerFinished()
    {
        UpdateQueuePositions(); // Sıradakiler öne geçsin
    }

    private void UpdateQueuePositions()
    {
        Customer[] customers = customerQueue.ToArray();
        for (int i = 0; i < customers.Length && i < queuePositions.Length; i++)
        {
            customers[i].SetTarget(queuePositions[i]);
        }
    }
}