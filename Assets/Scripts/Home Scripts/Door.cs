using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isOpen = false;
    public float openAngle = 90f;
    public float speed = 2f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    [SerializeField] private AudioClip doorSFX;
    [SerializeField] private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>().GetComponent<AudioSource>();
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.down * openAngle);
    }

    public void ToggleDoor()
    {
        audioSource.PlayOneShot(doorSFX);
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation));
    }

    private System.Collections.IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}