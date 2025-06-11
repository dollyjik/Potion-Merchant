using System;
using UnityEngine;

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
    [SerializeField] private bool canDrop = true;
    [SerializeField] private int layerNumber;


    private void Start()
    {
        playerCam = FindAnyObjectByType<PlayerCam>();
        moneyManager = FindAnyObjectByType<MoneyManager>( 0);
    }

    private void Update()
    {
        if (heldObj != null)
        {
            MoveObject();
            RotateObject();
        }

        if (playerCam.isUIOpened && Input.GetKeyDown(closeUIKey))
        {
            storeUI.SetActive(false);
            playerCam.isUIOpened = false;
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
                        CauldronScript cauldronScript = hit.transform.GetComponent<CauldronScript>();
                        cauldronScript.Craft();
                    }

                    if (hit.transform.gameObject.CompareTag("owlInteraction"))
                    {
                        storeUI.SetActive(true);
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
                        JarScript plantJar = plantStateMachine.PlantJar.GetComponent<JarScript>();
                        if (plantStateMachine.currentState == plantStateMachine.fruitState)
                        {
                            if (plantStateMachine.grownState != null)
                            {
                                plantStateMachine.ChangeState(plantStateMachine.grownState);
                                plantJar.AddIngredient();
                            }
                            else if (plantStateMachine.grownState == null)
                            {
                                plantStateMachine.ChangeState(plantStateMachine.growingState);
                                plantJar.AddIngredient();
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
            moneyManager.AddMoney();
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
                    if (hit.transform.gameObject.CompareTag("canPickUp"))
                    {
                        PickUpObject(hit.transform.gameObject);
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
            heldObj.layer = layerNumber;
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
