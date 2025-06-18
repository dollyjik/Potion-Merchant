using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Transform holdPos;
    [SerializeField] private PlayerCam playerCam;
    [SerializeField] private GameObject heldObj;
    [SerializeField] private Rigidbody heldObjRb;
    [SerializeField] private GameObject storeUI;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DayManager dayManager;
    [SerializeField] private CustomerQueueManager customerQueueManager;
    [SerializeField] public SeedData selectedSeed;
    [SerializeField] private AudioClip yawnSound;
    public AudioSource audioSource;
    [SerializeField] private UIManager uiManager; 

    
    [Header("Keybindings")]
    [SerializeField] private KeyCode firstInteractionKey = KeyCode.E;
    [SerializeField] private KeyCode secondInteractionKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode thirdInteractionKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode fourthInteractionKey = KeyCode.F;
    [SerializeField] private KeyCode denemeInteractionKey = KeyCode.Z;
    [SerializeField] private KeyCode moneyHackKey = KeyCode.M;
    [SerializeField] private KeyCode rotationKey = KeyCode.R;
    [SerializeField] private KeyCode closeUIKey = KeyCode.Escape;

    
    [Header("Variables")]
    [SerializeField] private float throwForce;
    [SerializeField] private float pickUpRange;
    [SerializeField] private float itemRotationSpeed;
    [SerializeField] private bool isStoreUIOpened;
    [SerializeField] private bool canDrop = true;
    [SerializeField] private LayerMask _layerMask ;


    private void Start()
    {
        playerCam = FindAnyObjectByType<PlayerCam>();
        moneyManager = FindAnyObjectByType<MoneyManager>( 0);
        dialogueManager = FindAnyObjectByType<DialogueManager>( 0);
        dayManager = FindAnyObjectByType<DayManager>( 0);
        customerQueueManager = FindAnyObjectByType<CustomerQueueManager>( 0);
    }

    private void Update()
    {
        if (heldObj != null)
        {
            if (Input.GetMouseButtonDown(0) && heldObj.TryGetComponent<SeedSOHolder>(out SeedSOHolder seedSOHolder))
            {
                Collider heldCollider = heldObj.GetComponent<Collider>();
                bool originalState = heldCollider.enabled;
                heldCollider.enabled = false; // Geçici olarak collider'ı kapat

                RaycastHit hit;
                if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, pickUpRange))
                {
                    Debug.Log("Raycast hit: " + hit.collider.name); 

                    PlantingSpot plantingSpot = hit.collider.GetComponent<PlantingSpot>();

                    if (plantingSpot != null)
                    {
                        plantingSpot.Plant(heldObj.GetComponent<SeedSOHolder>().seedData.plantPrefab);
                        Debug.Log("Planted");
                    }
                    else
                    {
                        Debug.LogWarning("The object " + hit.collider.name + " does not have a PlantingSpot script!");
                    }
                }
                Destroy(heldObj);
                heldCollider.enabled = originalState; // Collider'ı eski haline getir
            }

            else
            {
                MoveObject();
                RotateObject();
            }
        }
        
        if (!uiManager.isEscapePanelOpen && Input.GetKeyDown(closeUIKey) && !isStoreUIOpened)
        {
            uiManager.escapePanel.SetActive(true);
            Time.timeScale = 0f;
            playerCam.isUIOpened = true;
            uiManager.isEscapePanelOpen = true;
        }
        
        else if (uiManager.isEscapePanelOpen && Input.GetKeyDown(closeUIKey) && !isStoreUIOpened)
        {
            uiManager.escapePanel.SetActive(false);
            Time.timeScale = 1f;
            playerCam.isUIOpened = false;
            uiManager.isEscapePanelOpen = false;
        }
        
        else if (playerCam.isUIOpened && Input.GetKeyDown(closeUIKey))
        {
            storeUI.SetActive(false);
            playerCam.isUIOpened = false;
        }
        
        if (dialogueManager.isDialogueOpen && Input.GetKeyDown(firstInteractionKey))
        {
            dialogueManager.DisplayNextSentence();
        }

        if (Input.GetKeyDown(firstInteractionKey))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.CompareTag("Cauldron"))
                    {
                        Debug.Log("Cauldron Craft");
                        CauldronScript cauldronScript = hit.transform.GetComponent<CauldronScript>();
                        Debug.Log(cauldronScript);
                        cauldronScript.Craft();
                    }

                    if (hit.transform.gameObject.CompareTag("Bath"))
                    {
                        Bath bathScript = hit.transform.gameObject.GetComponent<Bath>();
                        bathScript.TetikleEkranKarartma();
                        audioSource.PlayOneShot(yawnSound);
                        dayManager.FinishDay();
                        foreach (GameObject customer in GameObject.FindGameObjectsWithTag("Customer"))
                        {
                            Destroy(customer);
                        }
                    }
                    
                    if (hit.transform.gameObject.CompareTag("owlInteraction"))
                    {
                        storeUI.SetActive(true);
                        isStoreUIOpened = true;
                        playerCam.isUIOpened = true;
                    }

                    if (hit.transform.gameObject.CompareTag("Door"))
                    {
                        Door door = hit.transform.GetComponent<Door>();
                        if (door != null)
                        {
                            door.ToggleDoor();
                        }
                    }

                    if (hit.transform.gameObject.CompareTag("Jar"))
                    {
                        JarScript jarScript = hit.transform.GetComponent<JarScript>();
                        GameObject jarObject = jarScript.SpawnIngredient(holdPos);
                        PickUpObject(jarObject);
                    }

                    if (hit.transform.gameObject.CompareTag("Plant"))
                    {
                        PlantStateMachine plantStateMachine = hit.transform.GetComponent<PlantStateMachine>();
                        IngredientSOHolder ingredientSOHolder = hit.transform.GetComponent<IngredientSOHolder>();
                        if (plantStateMachine.currentState == plantStateMachine.fruitState)
                        {
                            if (plantStateMachine.grownState != null)
                            {
                                plantStateMachine.ChangeState(plantStateMachine.grownState);
                                plantStateMachine.AddIngredientToJar(ingredientSOHolder.ingredientSO);
                            }
                            else if (plantStateMachine.grownState == null)
                            {
                                plantStateMachine.ChangeState(plantStateMachine.growingState);
                                plantStateMachine.AddIngredientToJar(ingredientSOHolder.ingredientSO);
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(denemeInteractionKey))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.CompareTag("Jar"))
                    {
                        JarScript jarScript = hit.transform.GetComponent<JarScript>();
                        jarScript.AddIngredient();
                    }
                }
            }
        }
        
        if (Input.GetKeyDown(secondInteractionKey))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.CompareTag("Cauldron"))
                    {
                        CauldronScript cauldronScript = hit.transform.GetComponent<CauldronScript>();
                        cauldronScript.NextRecipe();
                    }
                }
            }
        }

        if (Input.GetKeyDown(moneyHackKey))
        {
            moneyManager.AddMoney(500);
        }
        
        if (Input.GetKeyDown(thirdInteractionKey))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.CompareTag("Cauldron"))
                    {
                        CauldronScript cauldronScript = hit.transform.GetComponent<CauldronScript>();
                        cauldronScript.PreviousRecipe();
                    }
                }
            }
        }
        
        if (Input.GetKeyDown(fourthInteractionKey))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.CompareTag("canPickUp") || hit.transform.gameObject.CompareTag("Potion"))
                    {
                        PickUpObject(hit.transform.gameObject);
                    }

                    if (hit.transform.gameObject.CompareTag("Cauldron"))
                    {
                        CauldronScript cauldronScript = hit.transform.GetComponent<CauldronScript>();
                        cauldronScript.ClearCauldron();
                    }
                }
            }
            else if (heldObj != null)
            {
                if (canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
    }
    
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = 8;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
    }
    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }
    void RotateObject()
    {
        if (Input.GetKey(rotationKey))
        {
            canDrop = false;
            playerCam.sensX = 0f;
            playerCam.sensY = 0f;

            float xaxisRotation = Input.GetAxis("Mouse X") * itemRotationSpeed;
            float yaxisRotation = Input.GetAxis("Mouse Y") * itemRotationSpeed;
            
            heldObj.transform.Rotate(Vector3.down, xaxisRotation);
            heldObj.transform.Rotate(Vector3.right, yaxisRotation);
        }
        else
        {
            playerCam.sensX = 400f;
            playerCam.sensY = 400f;
            canDrop = true;
        }
    }
    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }
    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 start = transform.position;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * pickUpRange;
        
        Gizmos.DrawRay(start, direction);
    }
}
