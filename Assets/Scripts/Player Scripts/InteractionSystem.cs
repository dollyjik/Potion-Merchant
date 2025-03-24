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
    
    [Header("Keybindings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;
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
    }

    private void Update()
    {
        if (playerCam.isUIOpened && Input.GetKeyDown(closeUIKey))
        {
            storeUI.SetActive(false);
            playerCam.isUIOpened = false;
        }
        if (Input.GetKeyDown(interactionKey))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.CompareTag("canPickUp"))
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                    else if (hit.transform.gameObject.CompareTag("owlInteraction"))
                    {
                        storeUI.SetActive(true);
                        playerCam.isUIOpened = true;
                    }
                }
            }
            else
            {
                if(canDrop)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
        if (heldObj != null)
        {
            MoveObject();
            RotateObject();
            if (Input.GetKeyDown(throwKey) && canDrop == true)
            {
                StopClipping();
                ThrowObject();
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
