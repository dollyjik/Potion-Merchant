using UnityEngine;

public class CanvasVisibilityController : MonoBehaviour
{
    [Header("References")]
    private Transform cameraTransform;
    private CanvasGroup canvasGroup; // Optional: for fading instead of instant show/hide

    [Header("Distance Settings")]
    [SerializeField] private float visibilityDistance = 10f;
    [SerializeField] private float fadeStartDistance = 8f;
    [SerializeField] private float fadeSpeed = 3f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private bool onlyRotateWhenInView = true;
    [SerializeField] private float maxViewAngle = 70f;

    [Header("Debug")]
    public bool debugMode = false;
    
    private bool isVisible = false;
    
    void Start()
    {
        cameraTransform = Camera.main.transform;
        
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null && GetComponent<Canvas>() != null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        UpdateVisibility();
    }
    
    void Update()
    {
        UpdateVisibility();
        
        if (isVisible && ShouldRotate())
        {
            RotateTowardsCamera();
        }
    }
    
    void UpdateVisibility()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, cameraTransform.position);
        
        if (distanceToPlayer <= visibilityDistance)
        {
            if (canvasGroup != null)
            {
                
                gameObject.GetComponent<Canvas>().enabled = true;
                canvasGroup.enabled = true;
                
                float targetAlpha = distanceToPlayer > fadeStartDistance 
                    ? 1 - ((distanceToPlayer - fadeStartDistance) / (visibilityDistance - fadeStartDistance))
                    : 1f;
                
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
                canvasGroup.interactable = canvasGroup.alpha > 0.5f;
                canvasGroup.blocksRaycasts = canvasGroup.alpha > 0.5f;
            }
            
            isVisible = true;
        }
        else
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
                
                if (canvasGroup.alpha < 0.01f)
                {
                    gameObject.GetComponent<Canvas>().enabled = false;
                    canvasGroup.enabled = false;
                    
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
            
            isVisible = canvasGroup != null ? canvasGroup.alpha > 0 : gameObject.activeSelf;
        }
    }
    
    bool ShouldRotate()
    {
        if (!onlyRotateWhenInView)
            return true;
            
        Vector3 directionToCanvas = transform.position - cameraTransform.position;
        float angle = Vector3.Angle(cameraTransform.forward, directionToCanvas);
        
        return angle < maxViewAngle;
    }
    
    void RotateTowardsCamera()
    {
        Vector3 directionToCamera = cameraTransform.position - transform.position;
        
        directionToCamera.y = 0;
        
        if (directionToCamera != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
            
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!debugMode) 
            return;
            
        Gizmos.color = new Color(0, 0, 150, 0.6f);
        Gizmos.DrawWireSphere(transform.position, visibilityDistance);
        
        Gizmos.color = new Color(150, 0, 0, 0.6f);
        Gizmos.DrawWireSphere(transform.position, fadeStartDistance);
        
        if (cameraTransform != null)
        {
            Gizmos.color = isVisible ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, cameraTransform.position);
            
            if (onlyRotateWhenInView)
            {
                Vector3 dirToCamera = cameraTransform.position - transform.position;
                float angle = Vector3.Angle(cameraTransform.forward, dirToCamera);
                Gizmos.color = angle < maxViewAngle ? Color.green : Color.yellow;
                Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * 3);
            }
        }
    }
}