using System;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private CustomerStorySO story;
    [SerializeField] private CustomerStorySO[] possibleStories;
    [SerializeField] private float patienceTime = 30f;
    public PotionSO wantedPotion;

    private Transform refPoint;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    [SerializeField] private bool hasStartedOrder = false;

    private NavMeshAgent agent;
    private Animator animator;
    private MoneyManager moneyManager;
    
    public AudioClip customerSound;
    public AudioClip moneySound;
    public AudioClip failSound;
    public AudioSource audioSource;
    
    public Transform currentTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        moneyManager = FindAnyObjectByType<MoneyManager>();
        story = possibleStories[UnityEngine.Random.Range(0, possibleStories.Length)];
        refPoint = GameObject.FindGameObjectWithTag("RefPoint").transform;
        CustomerQueueManager.Instance.JoinQueue(this);
    }

    public void StartOrder()
    {
        hasStartedOrder = true; // ✅ Sipariş başlatıldı olarak işaretle
        waitTimer = patienceTime;
        wantedPotion = story.wantedPotion;
        isWaiting = true;

        Debug.Log($"Starting order for customer: {gameObject.name}"); // ✅ Debug log
        DialogueManager.Instance.StartDialogue(story.storyLines);
        audioSource.PlayOneShot(customerSound);
    }

    private void Update()
    {
        // Bekleme süresi
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                FailOrder();
            }
        }

        // Hedefe yürüme
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
            animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
            
            if (currentTarget == CustomerQueueManager.Instance.OrderPoint && 
                ReachedTarget() && !hasStartedOrder)
            {
                StartOrder();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // Oyuncuya bakma
        if (ReachedTarget() && refPoint != null && isWaiting)
        {
            Vector3 direction = (refPoint.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    public bool ReachedTarget()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public void RecivePotion(PotionSO potionGiven)
    {
        if (!isWaiting) return;

        if (potionGiven.PotionID == wantedPotion.PotionID)
        {
            SuccessOrder();
        }
        else
        {
            FailOrder();
        }
    }

    private void SuccessOrder()
    {
        moneyManager.AddMoney(wantedPotion.PotionPrice);
        isWaiting = false;
        hasStartedOrder = false;
        Debug.Log("Success");
        
        audioSource.PlayOneShot(moneySound);
        CustomerQueueManager.Instance.NotifyCustomerFinished(); // ✅ Müşteri işini bitirdi
        Leave();
    }

    private void FailOrder()
    {
        isWaiting = false;
        hasStartedOrder = false;
        Debug.Log("Fail");
        audioSource.PlayOneShot(failSound);
        CustomerQueueManager.Instance.NotifyCustomerFinished(); // ✅ Müşteri işini bitirdi
        Leave();
    }

    private void Leave()
    {
        animator.SetBool("isWalking", true);
        SetTarget(CustomerQueueManager.Instance.ExitPoint);
        Invoke(nameof(DestroySelf), 2.5f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}