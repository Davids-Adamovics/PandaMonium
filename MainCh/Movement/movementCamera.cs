using UnityEngine;

public class HUDCameraController : MonoBehaviour
{
    public float rotationAmount = 1f;
    public float rotationSpeed = 2f;
    public float returnSpeed = 1f;

    private Quaternion originalRotation;
    private float horizontalInput;
    private float verticalInput;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        Quaternion targetRotation = originalRotation * Quaternion.Euler(-mouseY * rotationAmount - verticalInput * rotationAmount, mouseX * rotationAmount - horizontalInput * rotationAmount, 0f);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (Mathf.Abs(horizontalInput) < 0.1f && Mathf.Abs(verticalInput) < 0.1f && Mathf.Abs(mouseX) < 0.1f && Mathf.Abs(mouseY) < 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * returnSpeed);
        }
    }
}
