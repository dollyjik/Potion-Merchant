using UnityEngine;

public class WindowScript : MonoBehaviour
{
    private bool isOpen = false;
    public float openAngle = 90f;
    public float speed = 4f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.down * openAngle);
    }

    public void ToggleWindow()
    {
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateWindow(isOpen ? openRotation : closedRotation));
    }

    private System.Collections.IEnumerator RotateWindow(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
